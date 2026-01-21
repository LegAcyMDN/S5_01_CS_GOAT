using System.Dynamic;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using Shared.DTO;
using Shared.DTO.Helpers;
using Shared.Enum;

namespace S5_01_App_CS_GOAT.Models.DataManager;

/// <summary>
/// Manages user account operations including creation, authentication, updates, and profile management
/// </summary>
public class UserManager : CrudRepository<User, int>, IUserRepository
{
    protected readonly IConfiguration _configuration;
    protected readonly IMapper _mapper;

    public UserManager(CSGOATDbContext context, IMapper mapper, IConfiguration configuration) : base(context)
    {
        _mapper = mapper;
        _configuration = configuration;
    }

    /// <summary>
    /// Updates user profile information including email, phone, 2FA method, display name, and password
    /// </summary>
    /// <param name="existing">The existing user entity to update</param>
    /// <param name="userDTO">The new user details from the request</param>
    /// <exception cref="InvalidOperationException">Thrown if email/phone already in use, 2FA validation fails, or password verification fails</exception>
    /// <remarks>
    /// Complex validation rules:
    /// - Email/phone uniqueness checked if changed
    /// - Email/phone verification reset when changed (user must re-verify)
    /// - 2FA method changes require password verification
    /// - 2FA method must have verified contact method (cannot enable phone 2FA without verified phone)
    /// - Password changes require current password verification
    /// </remarks>
    public async Task UpdateUserDetails(User existing, UpdateUserDTO userDTO)
    {
        _ = _context.Set<User>().Attach(existing);

        bool needPasswordCheck = false;

        if (userDTO.Email == null && userDTO.Phone == null)
        {
            userDTO.Email = existing.Email;
            userDTO.Phone = existing.Phone;
        }
        existing.DisplayName = userDTO.DisplayName ?? existing.DisplayName;
        existing.Seed = userDTO.Seed ?? existing.Seed;

        if (existing.TwoFaIsEmail && userDTO.TwoFA != TwoFAmethod.Email)
        {
            needPasswordCheck = true;
            existing.TwoFaIsEmail = false;
            existing.TwoFaIsPhone = userDTO.TwoFA == TwoFAmethod.Phone;
        }
        else if (existing.TwoFaIsPhone && userDTO.TwoFA != TwoFAmethod.Phone)
        {
            needPasswordCheck = true;
            existing.TwoFaIsPhone = false;
            existing.TwoFaIsEmail = userDTO.TwoFA == TwoFAmethod.Email;
        }

        if (existing.Email != userDTO.Email)
        {
            if (userDTO.Email != null && await GetByEmail(userDTO.Email) != null)
            {
                throw new InvalidOperationException("Email is already in use.");
            }

            existing.Email = userDTO.Email;
            existing.EmailVerifiedOn = null;
        }
        if (existing.Phone != userDTO.Phone)
        {
            if (userDTO.Phone != null && await GetByPhone(userDTO.Phone) != null)
            {
                throw new InvalidOperationException("Phone is already in use.");
            }

            existing.Phone = userDTO.Phone;
            existing.PhoneVerifiedOn = null;
        }

        if (existing.TwoFaIsEmail && existing.EmailVerifiedOn == null)
        {
            throw new InvalidOperationException("Cannot use email 2FA without a verified email.");
        }

        if (existing.TwoFaIsPhone && existing.PhoneVerifiedOn == null)
        {
            throw new InvalidOperationException("Cannot use phone 2FA without a verified phone.");
        }

        if (needPasswordCheck || userDTO.NewPassword != null)
        {
            if (string.IsNullOrEmpty(userDTO.OldPassword))
            {
                throw new InvalidOperationException("Current password is required to update sensitive information.");
            }

            bool? goodPassword = SecurityService.VerifyPassword(
                userDTO.OldPassword,
                existing.HashPassword,
                existing.SaltPassword
            );
            if (goodPassword != true)
            {
                throw new InvalidOperationException("Current password is incorrect.");
            }

            if (userDTO.NewPassword != null)
            {
                if (!existing.TrySetPassword(userDTO.NewPassword))
                {
                    throw new InvalidOperationException("New password does not meet complexity requirements.");
                }
            }
        }

        _ = await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Creates a new user account with login credentials and contact information
    /// </summary>
    /// <param name="newAccount">Account creation parameters including login, password, email, phone</param>
    /// <returns>The newly created User entity</returns>
    /// <exception cref="InvalidOperationException">Thrown if login/email/phone already exists or password doesn't meet requirements</exception>
    /// <remarks>
    /// Validation and initialization:
    /// - Login must be unique
    /// - At least one contact method (email or phone) must be provided
    /// - Email and phone must be unique if provided
    /// - Password must meet complexity requirements (enforced by User.TrySetPassword)
    /// - Initializes new user with notification settings, limits, and welcome promo code
    /// - Uses database transaction for atomicity
    /// </remarks>
    public async Task<User> CreateUser(CreateUserDTO newAccount)
    {
        if (await GetByLogin(newAccount.Login) != null)
        {
            throw new InvalidOperationException("Login is already in use.");
        }

        if (newAccount.Email == null && newAccount.Phone == null)
        {
            throw new InvalidOperationException("At least one contact method (email or phone) must be provided.");
        }

        if (newAccount.Email != null && await GetByEmail(newAccount.Email) != null)
        {
            throw new InvalidOperationException("Email is already in use.");
        }

        if (newAccount.Phone != null && await GetByPhone(newAccount.Phone) != null)
        {
            throw new InvalidOperationException("Phone is already in use.");
        }

        var newUser = new User
        {
            Login = newAccount.Login,
            DisplayName = newAccount.DisplayName,
            Email = newAccount.Email,
            Phone = newAccount.Phone
        };
        if (!newUser.TrySetPassword(newAccount.Password))
        {
            throw new InvalidOperationException("Password does not meet complexity requirements.");
        }

        await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
        _ = await _context.Set<User>().AddAsync(newUser);
        _ = await _context.SaveChangesAsync();
        await InitializeUser(newUser);
        _ = await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return newUser;
    }

    /// <summary>
    /// Initializes a new user with default notification settings, limits, and welcome promo code
    /// </summary>
    /// <param name="user">The user entity to initialize</param>
    /// <remarks>
    /// Creates:
    /// - NotificationSetting entries for each NotificationType
    /// - Limit entries for each LimitType
    /// - Welcome promo code (WELCOME): 25% discount, expires in 7 days, 1 use
    /// </remarks>
    public async Task InitializeUser(User user)
    {
        List<NotificationType> nTypes = await _context.Set<NotificationType>().ToListAsync();
        foreach (NotificationType nType in nTypes)
        {
            var setting = new NotificationSetting
            {
                UserId = user.UserId,
                NotificationType = nType
            };
            _ = await _context.Set<NotificationSetting>().AddAsync(setting);
        }

        List<LimitType> limitTypes = await _context.Set<LimitType>().ToListAsync();
        foreach (LimitType lType in limitTypes)
        {
            var limit = new Limit
            {
                UserId = user.UserId,
                LimitType = lType
            };
            _ = await _context.Set<Limit>().AddAsync(limit);
        }

        var newUserPromoCode = new PromoCode
        {
            UserId = user.UserId,
            RemainingUses = 1,
            Code = "WELCOME",
            DiscountPercentage = 25,
            DiscountAmount = 0,
            ExpiryDate = DateTime.Now.AddDays(7)
        };
        _ = await _context.Set<PromoCode>().AddAsync(newUserPromoCode);
    }

    /// <summary>
    /// Authenticates user with login credentials (password verification)
    /// </summary>
    /// <param name="loginDTO">Login parameters with identifier (login/email/phone) and password</param>
    /// <returns>User entity if authentication succeeds, null otherwise</returns>
    /// <remarks>
    /// Process:
    /// 1. Looks up user by identifier (login/email/phone - case-insensitive)
    /// 2. Verifies password using SecurityService.VerifyPassword
    /// 3. Returns user if both succeed, null if either fails
    /// </remarks>
    public async Task<User?> Login(LoginDTO loginDTO)
    {
        User? user = await GetByIdentifier(loginDTO.Identifier);
        if (user == null)
        {
            return null;
        }

        bool? goodPassword = SecurityService.VerifyPassword(
            loginDTO.Password,
            user.HashPassword,
            user.SaltPassword);

        return goodPassword != true ? null : user;
    }

    /// <summary>
    /// Recalls user session using a persistent remember token (token type 1)
    /// </summary>
    /// <param name="rememberDTO">Token details for session recall</param>
    /// <returns>User entity if token is valid, null otherwise</returns>
    /// <remarks>
    /// Validation:
    /// - Token must exist and match provided value
    /// - Token must be type 1 (remember token)
    /// - Token must not be expired
    /// - Expired tokens are automatically deleted from database
    /// </remarks>
    public async Task<User?> Recall(TokenDTO rememberDTO)
    {
        Token? token = await _context.Set<Token>().FindAsync(rememberDTO.TokenId);
        if (
            token == null
            || token.TokenValue != rememberDTO.TokenValue
            || token.UserId != rememberDTO.UserId
            || token.TokenTypeId != 1
        )
        {
            return null;
        }

        if (token.TokenExpiry <= DateTime.Now)
        {
            _ = _context.Set<Token>().Remove(token);
            _ = await _context.SaveChangesAsync();
            return null;
        }

        User? user = await _context.Set<User>()
            .FirstOrDefaultAsync(u => u.UserId == token.UserId);

        return user;
    }

    /// <summary>
    /// Generates authentication tokens (JWT and optional remember token) after successful login
    /// </summary>
    /// <param name="user">The authenticated user</param>
    /// <param name="config">Configuration for JWT generation</param>
    /// <param name="remember">Optional remember token duration in days (null = no remember token)</param>
    /// <returns>AuthDTO with JWT token and optional remember token</returns>
    /// <remarks>
    /// Operations:
    /// - Generates JWT token using JwtService (expires in 30 days)
    /// - Creates remember token if remember parameter > 0
    /// - Updates LastLogin timestamp
    /// - Returns JWT and remember token for client storage
    /// </remarks>
    public async Task<AuthDTO> Auth(User user, IConfiguration config, int? remember = null)
    {
        _ = _context.Set<User>().Attach(user);
        string jwtToken = JwtService.GenerateJwtToken(user, config);
        Token? rememberToken = null;
        if (remember != null && remember > 0)
        {
            DateTime tokenExpiry = DateTime.Now.AddDays(remember.Value);
            rememberToken = new Token
            {
                UserId = user.UserId,
                TokenTypeId = 1,
                TokenExpiry = tokenExpiry
            };
        }
        var authDTO = new AuthDTO
        {
            UserId = user.UserId,
            DisplayName = user.DisplayName,
            JwtToken = jwtToken
        };
        if (rememberToken != null)
        {
            _ = await _context.Set<Token>().AddAsync(rememberToken);
        }
        user.LastLogin = DateTime.Now;
        _ = await _context.SaveChangesAsync();
        authDTO.RememberToken = rememberToken != null!
            ? _mapper.Map<TokenDTO>(rememberToken)
            : null;
        return authDTO;
    }

    /// <summary>
    /// Retrieves user by login name (case-insensitive)
    /// </summary>
    public async Task<User?> GetByLogin(string login)
    {
        return await _context.Set<User>()
            .FirstOrDefaultAsync(
                u => u.Login != null && u.Login.ToLower() == login.ToLower()
            );
    }

    /// <summary>
    /// Retrieves user by email address (case-insensitive)
    /// </summary>
    public async Task<User?> GetByEmail(string email)
    {
        return await _context.Set<User>()
            .FirstOrDefaultAsync(
                u => u.Email != null && u.Email.ToLower() == email.ToLower()
            );
    }

    /// <summary>
    /// Retrieves user by phone number (case-insensitive)
    /// </summary>
    public async Task<User?> GetByPhone(string phone)
    {
        return await _context.Set<User>()
            .FirstOrDefaultAsync(
                u => u.Phone != null && u.Phone.ToLower() == phone.ToLower()
            );
    }

    /// <summary>
    /// Retrieves user by flexible identifier (login, email, or phone)
    /// </summary>
    /// <remarks>
    /// Tries in order:
    /// 1. Login (exact match, case-insensitive)
    /// 2. Email (exact match, case-insensitive)
    /// 3. Phone (exact match, case-insensitive)
    /// </remarks>
    public async Task<User?> GetByIdentifier(string identifier)
    {
        User? user = await GetByLogin(identifier);
        if (user != null)
        {
            return user;
        }

        user = await GetByEmail(identifier);
        if (user != null)
        {
            return user;
        }

        user = await GetByPhone(identifier);
        return user;
    }

    /// <summary>
    /// Get user by SteamID
    /// </summary>
    /// <param name="steamId">Steam ID (64-bit)</param>
    /// <returns>User if found, null otherwise</returns>
    public async Task<User?> GetBySteamIdAsync(string steamId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.SteamId == steamId && u.DeletedOn == null);
    }

    /// <summary>
    /// Create or update user from Steam authentication
    /// </summary>
    /// <param name="steamAuthDTO">Steam authentication data</param>
    /// <returns>The created or updated user</returns>
    /// <summary>
    /// Create or update user from Steam authentication
    /// </summary>
    /// <summary>
    /// Creates or updates user from Steam authentication
    /// </summary>
    /// <param name="steamAuthDTO">Steam authentication data from Steam API</param>
    /// <returns>The created or updated user</returns>
    /// <remarks>
    /// Two scenarios:
    /// - Existing user: Updates DisplayName and LastLogin
    /// - New user: Creates account with random secure login/password, no email/phone (Steam only)
    /// 
    /// Steam-only users have:
    /// - Login format: "steam_{SteamId}"
    /// - Random 64-character password (user cannot login without Steam)
    /// - No email/phone initially
    /// - 2FA disabled
    /// </remarks>
    public async Task<User> AuthenticateWithSteam(SteamAuthDTO steamAuthDTO)
    {
        // Check if user already exists
        User? existingUser = await GetBySteamIdAsync(steamAuthDTO.SteamId);

        if (existingUser != null)
        {
            // Update existing user
            existingUser.DisplayName = steamAuthDTO.Username;
            existingUser.LastLogin = DateTime.UtcNow;

            _ = _context.Users.Update(existingUser);
            _ = await _context.SaveChangesAsync();

            return existingUser;
        }

        // Create new user with random secure credentials
        string randomSalt = SecurityService.GenerateToken(32);
        string randomPassword = SecurityService.GenerateToken(64);
        string hashedPassword = SecurityService.HashAndSalt(randomPassword, randomSalt);

        var newUser = new User
        {
            SteamId = steamAuthDTO.SteamId,
            Login = $"steam_{steamAuthDTO.SteamId}",
            DisplayName = steamAuthDTO.Username,
            Email = null,
            Phone = null,
            SaltPassword = randomSalt,
            HashPassword = hashedPassword,
            TwoFaIsPhone = false,
            TwoFaIsEmail = false,
            IsAdmin = false,
            CreationDate = DateTime.UtcNow,
            LastLogin = DateTime.UtcNow,
            Wallet = 0.0,
            DeletedOn = null,
            Seed = SecurityService.GenerateSeed(16),
            Nonce = 0
        };

        _ = await _context.Users.AddAsync(newUser);
        _ = await _context.SaveChangesAsync();

        return newUser;
    }


    /// <summary>
    /// Initiates password reset process by sending reset token via email or SMS
    /// </summary>
    /// <param name="identifier">User identifier (login, email, or phone)</param>
    /// <param name="url">Base URL for password reset link</param>
    /// <param name="preferMail">If true, sends via email when available; otherwise SMS</param>
    /// <returns>HTTP status code (200 success, 404 user not found, 429 too many requests, 400 no contact method)</returns>
    /// <remarks>
    /// Rate limiting:
    /// - Prevents multiple reset requests within 15 minutes
    /// - Returns 429 if existing unexpired reset token exists
    /// 
    /// Token details:
    /// - Type: 2 (password reset)
    /// - Duration: 15 minutes
    /// - Length: 32 chars for email, 16 chars for SMS
    /// 
    /// Message format: Bilingual (English/French) with reset link
    /// </remarks>
    public async Task<int> StartResetPassword(string identifier, string url, bool preferMail = true)
    {
        Console.WriteLine($"Starting password reset for identifier: {identifier}, url: {url}, preferMail: {preferMail}");
        User? user = await GetByIdentifier(identifier);
        if (user == null)
        {
            return StatusCodes.Status404NotFound;
        }

        IEnumerable<Token> existingTokens = _context.Set<Token>()
            .Where(t => t.UserId == user.UserId && t.TokenTypeId == 2 && t.TokenExpiry > DateTime.Now);
        if (existingTokens.Any())
        {
            return StatusCodes.Status429TooManyRequests;
        }

        using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
        var resetToken = new Token
        {
            UserId = user.UserId,
            TokenTypeId = 2,
            TokenExpiry = DateTime.Now.AddMinutes(15),
            TokenValue = SecurityService.GenerateSeed(preferMail ? 32 : 16)
        };
        _ = await _context.Set<Token>().AddAsync(resetToken);
        _ = await _context.SaveChangesAsync();

        var message = new Message(_configuration, user)
        {
            Text = $"\n\nA password reset request has been received for your CS:GOAT account." +
                $"\nFollow the link below if you have forgotten your password." +
                $"\nIf you did not initiate this request, you can ignore it." +
                $"\n{url}?identifier={identifier}&code={resetToken.TokenValue}",
            Subject = "Réinitialisation du mot de passe CS:GOAT / CS:GOAT Password Reset",
            Html = $"<p>Une demande de réinitialisation de votre mot de passe à été reçue pour votre compte CS:GOAT.<br>" +
                $"Suivez le lien ci-dessous si vous avez oublié votre mot de passe.<br>" +
                $"Si vous n'êtes pas à l'origine de cette demande, vous pouvez l'ignorer.</p>" +
                $"<p>A password reset request has been received for your CS:GOAT account.<br>" +
                $"Follow the link below if you have forgotten your password.<br>" +
                $"If you did not initiate this request, you can ignore it.</p>" +
                $"<p><a href=\"{url}?identifier={identifier}&code={resetToken.TokenValue}\">" +
                $"{url}?identifier={identifier}&code={resetToken.TokenValue}</a></p>"
        };
        if ((preferMail && !string.IsNullOrEmpty(user.Email)) || string.IsNullOrEmpty(user.Phone))
        {
            HttpResponseMessage response = await message.SendMailAsync();
            if (!response.IsSuccessStatusCode)
            {
                await transaction.RollbackAsync();
            }
            else
            {
                await transaction.CommitAsync();
            }

            return (int)response.StatusCode;
        }
        else if (!string.IsNullOrEmpty(user.Phone))
        {
            HttpResponseMessage response = await message.SendSmsAsync();
            if (!response.IsSuccessStatusCode)
            {
                await transaction.RollbackAsync();
            }
            else
            {
                await transaction.CommitAsync();
            }

            return (int)response.StatusCode;
        }
        await transaction.RollbackAsync();
        return StatusCodes.Status400BadRequest;
    }

    /// <summary>
    /// Completes password reset by validating reset token and setting new password
    /// </summary>
    /// <param name="identifier">User identifier (login, email, or phone)</param>
    /// <param name="code">The reset token from the reset email/SMS</param>
    /// <returns>Tuple of (HTTP status code, new password if successful)</returns>
    /// <remarks>
    /// Validation:
    /// - User must exist
    /// - Token must exist, match type 2, not be expired
    /// - Token is deleted after use (one-time use)
    /// 
    /// Return values:
    /// - (200, new_password): Success - user should login with new password
    /// - (404, null): User not found
    /// - (400, null): Invalid or expired token
    /// - (500, null): Password generation failed
    /// 
    /// Generated password: Random 12-character token
    /// </remarks>
    public async Task<Tuple<int, string?>> EndResetPassword(string identifier, string code)
    {
        User? user = await GetByIdentifier(identifier);
        if (user == null)
        {
            return new Tuple<int, string?>(StatusCodes.Status404NotFound, null);
        }

        Token? token = await _context.Set<Token>()
            .FirstOrDefaultAsync(t =>
                t.UserId == user.UserId &&
                t.TokenTypeId == 2 &&
                t.TokenValue == code &&
                t.TokenExpiry > DateTime.Now);
        if (token == null)
        {
            return new Tuple<int, string?>(StatusCodes.Status400BadRequest, null);
        }

        string newPassword = SecurityService.GenerateSeed(12);
        if (!user.TrySetPassword(newPassword, true))
        {
            return new Tuple<int, string?>(StatusCodes.Status500InternalServerError, null);
        }

        _ = _context.Set<Token>().Remove(token);
        _ = await _context.SaveChangesAsync();
        return new Tuple<int, string?>(StatusCodes.Status200OK, newPassword);
    }

    /// <summary>
    /// Exports complete user data for GDPR compliance (data portability)
    /// </summary>
    /// <param name="userId">The user ID to export</param>
    /// <returns>Dynamic object with user data and related transactions, null if user not found</returns>
    /// <remarks>
    /// Exported data includes:
    /// - User profile (name, settings) - excludes password hashes
    /// - Notification settings and preferences
    /// - Limits and bans
    /// - Favorite cases list
    /// - Inventory items with full details
    /// - All transactions (item, money, upgrade results)
    /// - Fair random sessions for verification
    /// 
    /// Sensitive data excluded:
    /// - SaltPassword and HashPassword (security)
    /// - Email verification status (internal)
    /// </remarks>
    public async Task<object?> ExportUserDataAsync(int userId)
    {
        QueryOptions<User> userOptions = new QueryOptions<User>()
            .Before("NotificationSettings.NotificationType",
                "Limits.LimitType", "Bans.BanType")
            .Before(u => u.Favorites);
        User? user = await GetByIdAsync(userId, userOptions);
        if (user == null)
        {
            return null;
        }

        dynamic exportUser = new ExpandoObject();
        var exportUserDict = (IDictionary<string, object?>)exportUser;
        foreach (System.Reflection.PropertyInfo prop in user.GetType().GetProperties())
        {
            Type propertyType = prop.PropertyType;
            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(propertyType) && propertyType != typeof(string))
            {
                continue; // Skip navigation properties (IEnumerable, ICollection, complex types)
            }

            if (typeof(User)?.Namespace != null && (propertyType.Namespace?.StartsWith(typeof(User).Namespace!) ?? false))
            {
                continue; // Skip custom entities / navigation properties (EF models)
            }

            exportUserDict[prop.Name] = prop.GetValue(user);
        }
        exportUser.SaltPassword = null;
        exportUser.HashPassword = null;

        List<InventoryItem> inventoryItems = await _context.Set<InventoryItem>()
            .Include(ii => ii.Wear).ThenInclude(w => w.WearType)
            .Include(ii => ii.Wear).ThenInclude(w => w.Skin).ThenInclude(s => s.Rarity)
            .Include(ii => ii.Wear).ThenInclude(w => w.Skin).ThenInclude(s => s.Item).ThenInclude(i => i.ItemType)
            .Where(ii => ii.UserId == userId).ToListAsync();
        List<RandomTransaction> randTrans = await _context.Set<RandomTransaction>()
            .Where(rt => rt.UserId == userId).Include(rt => rt.FairRandom).ToListAsync();
        List<ItemTransaction> itemTrans = await _context.Set<ItemTransaction>()
            .Where(it => !randTrans.Select(randTrans => randTrans.TransactionId).Contains(it.TransactionId)
                && it.UserId == userId).ToListAsync();
        List<MoneyTransaction> moneyTrans = await _context.Set<MoneyTransaction>()
            .Where(mt => mt.UserId == userId).ToListAsync();
        List<UpgradeResult> upgradeResults = await _context.Set<UpgradeResult>()
            .Where(ur => inventoryItems.Select(ii => ii.InventoryItemId).Contains(ur.InventoryItemId))
            .Include(ur => ur.FairRandom).ToListAsync();

        exportUser.NotificationSettings = _mapper.Map<List<NotificationSettingDTO>>(user.NotificationSettings);
        exportUser.Limits = _mapper.Map<List<LimitDTO>>(user.Limits);
        exportUser.Bans = _mapper.Map<List<BanDTO>>(user.Bans);
        exportUser.Favorites = user.Favorites.Select(f => f.CaseId).ToList();
        exportUser.InventoryItems = _mapper.Map<List<InventoryItemDTO>>(inventoryItems);
        exportUser.ItemTransactions = _mapper.Map<List<ItemTransactionDTO>>(itemTrans);
        exportUser.MoneyTransactions = _mapper.Map<List<MoneyTransactionDTO>>(moneyTrans);
        exportUser.UpgradeResults = _mapper.Map<List<UpgradeResultDTO>>(upgradeResults);
        exportUser.RandomTransactions = randTrans.Select(rt => new
        {
            rt.TransactionId,
            rt.InventoryItemId,
            rt.TransactionDate,
            rt.WalletValue,
            rt.CancelledOn,
            rt.CaseId,
            FairRandom = _mapper.Map<FairRandomDTO>(rt.FairRandom)
        }).ToList();

        return exportUser;
    }
}