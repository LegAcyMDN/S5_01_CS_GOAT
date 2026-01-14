using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using Shared.DTO;
using Shared.DTO.Helpers;
using Shared.Enum;
using System.Dynamic;

namespace S5_01_App_CS_GOAT.Models.DataManager;

public class UserManager : CrudRepository<User, int>, IUserRepository
{
    protected readonly IConfiguration _configuration;
    protected readonly IMapper _mapper;

    public UserManager(CSGOATDbContext context, IMapper mapper, IConfiguration configuration) : base(context)
    {
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task UpdateUserDetails(User existing, UpdateUserDTO userDTO)
    {
        _context.Set<User>().Attach(existing);

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
            if (userDTO.Email != null && await this.GetByEmail(userDTO.Email) != null)
                throw new InvalidOperationException("Email is already in use.");
            existing.Email = userDTO.Email;
            existing.EmailVerifiedOn = null;
        }
        if (existing.Phone != userDTO.Phone)
        {
            if (userDTO.Phone != null && await this.GetByPhone(userDTO.Phone) != null)
                throw new InvalidOperationException("Phone is already in use.");
            existing.Phone = userDTO.Phone;
            existing.PhoneVerifiedOn = null;
        }

        if (existing.TwoFaIsEmail && existing.EmailVerifiedOn == null)
            throw new InvalidOperationException("Cannot use email 2FA without a verified email.");
        if (existing.TwoFaIsPhone && existing.PhoneVerifiedOn == null)
            throw new InvalidOperationException("Cannot use phone 2FA without a verified phone.");

        if (needPasswordCheck || userDTO.NewPassword != null)
        {
            if (string.IsNullOrEmpty(userDTO.OldPassword))
                throw new InvalidOperationException("Current password is required to update sensitive information.");
            
            bool? goodPassword = SecurityService.VerifyPassword(
                userDTO.OldPassword,
                existing.HashPassword,
                existing.SaltPassword
            );
            if (goodPassword != true)
                throw new InvalidOperationException("Current password is incorrect.");

            if (userDTO.NewPassword != null)
                if (!existing.TrySetPassword(userDTO.NewPassword))
                    throw new InvalidOperationException("New password does not meet complexity requirements.");
        }

        await _context.SaveChangesAsync();
    }

    public async Task<User> CreateUser(CreateUserDTO newAccount)
    {
        if (await GetByLogin(newAccount.Login) != null)
            throw new InvalidOperationException("Login is already in use.");
        if (newAccount.Email == null && newAccount.Phone == null)
            throw new InvalidOperationException("At least one contact method (email or phone) must be provided.");
        if (newAccount.Email != null && await GetByEmail(newAccount.Email) != null)
            throw new InvalidOperationException("Email is already in use.");
        if (newAccount.Phone != null && await GetByPhone(newAccount.Phone) != null)
            throw new InvalidOperationException("Phone is already in use.");

        User newUser = new User
        {
            Login = newAccount.Login,
            DisplayName = newAccount.DisplayName,
            Email = newAccount.Email,
            Phone = newAccount.Phone
        };
        if (!newUser.TrySetPassword(newAccount.Password))
            throw new InvalidOperationException("Password does not meet complexity requirements.");

        await using var transaction = await _context.Database.BeginTransactionAsync();
        await _context.Set<User>().AddAsync(newUser);
        await _context.SaveChangesAsync();
        await InitializeUser(newUser);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return newUser;
    }

    public async Task InitializeUser(User user)
    {
        List<NotificationType> nTypes = await _context.Set<NotificationType>().ToListAsync();
        foreach (var nType in nTypes)
        {
            NotificationSetting setting = new NotificationSetting
            {
                UserId = user.UserId,
                NotificationType = nType
            };
            await _context.Set<NotificationSetting>().AddAsync(setting);
        }

        List<LimitType> limitTypes = await _context.Set<LimitType>().ToListAsync();
        foreach (var lType in limitTypes)
        {
            Limit limit = new Limit
            {
                UserId = user.UserId,
                LimitType = lType
            };
            await _context.Set<Limit>().AddAsync(limit);
        }

        PromoCode newUserPromoCode = new PromoCode
        {
            UserId = user.UserId,
            RemainingUses = 1,
            Code = "WELCOME",
            DiscountPercentage = 25,
            DiscountAmount = 0,
            ExpiryDate = DateTime.Now.AddDays(7)
        };
        await _context.Set<PromoCode>().AddAsync(newUserPromoCode);
    }

    public async Task<User?> Login(LoginDTO loginDTO)
    {
        User? user = await this.GetByIdentifier(loginDTO.Identifier);
        if (user == null) return null;

        bool? goodPassword = SecurityService.VerifyPassword(
            loginDTO.Password,
            user.HashPassword,
            user.SaltPassword);

        if (goodPassword != true) return null;

        return user;
    }

    public async Task<User?> Recall(TokenDTO rememberDTO)
    {
        Token? token = await _context.Set<Token>().FindAsync(rememberDTO.TokenId);
        if (
            token == null
            || token.TokenValue != rememberDTO.TokenValue
            || token.UserId != rememberDTO.UserId
            || token.TokenTypeId != 1
        ) return null;

        if (token.TokenExpiry <= DateTime.Now)
        {
            _context.Set<Token>().Remove(token);
            await _context.SaveChangesAsync();
            return null;
        }

        User? user = await _context.Set<User>()
            .FirstOrDefaultAsync(u => u.UserId == token.UserId);

        return user;
    }

    public async Task<AuthDTO> Auth(User user, IConfiguration config, int? remember = null)
    {
        _context.Set<User>().Attach(user);
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
        AuthDTO authDTO = new AuthDTO
        {
            UserId = user.UserId,
            DisplayName = user.DisplayName,
            JwtToken = jwtToken
        };
        if (rememberToken != null)
        {
            await _context.Set<Token>().AddAsync(rememberToken);
        }
        user.LastLogin = DateTime.Now;
        await _context.SaveChangesAsync();
        authDTO.RememberToken = rememberToken != null!
            ? _mapper.Map<TokenDTO>(rememberToken)
            : null;
        return authDTO;
    }

    public async Task<User?> GetByLogin(string login)
    {
        return await _context.Set<User>()
            .FirstOrDefaultAsync(
                u => u.Login != null && u.Login.ToLower() == login.ToLower()
            );
    }

    public async Task<User?> GetByEmail(string email)
    {
        return await _context.Set<User>()
            .FirstOrDefaultAsync(
                u => u.Email != null && u.Email.ToLower() == email.ToLower()
            );
    }

    public async Task<User?> GetByPhone(string phone)
    {
        return await _context.Set<User>()
            .FirstOrDefaultAsync(
                u => u.Phone != null && u.Phone.ToLower() == phone.ToLower()
            );
    }

    public async Task<User?> GetByIdentifier(string identifier)
    {
        User? user = await GetByLogin(identifier);
        if (user != null) return user;
        user = await GetByEmail(identifier);
        if (user != null) return user;
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
        public async Task<User> AuthenticateWithSteam(SteamAuthDTO steamAuthDTO)
        {
            // Check if user already exists
            var existingUser = await GetBySteamIdAsync(steamAuthDTO.SteamId);
    
            if (existingUser != null)
            {
                // Update existing user
                existingUser.DisplayName = steamAuthDTO.Username;
                existingUser.LastLogin = DateTime.UtcNow;
        
                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();
        
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
    
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
    
            return newUser;
        }


    public async Task<int> StartResetPassword(string identifier, string url, bool preferMail = true)
    {
        Console.WriteLine($"Starting password reset for identifier: {identifier}, url: {url}, preferMail: {preferMail}");
        User? user = await GetByIdentifier(identifier);
        if (user == null) return StatusCodes.Status404NotFound;
        IEnumerable<Token> existingTokens = _context.Set<Token>()
            .Where(t => t.UserId == user.UserId && t.TokenTypeId == 2 && t.TokenExpiry > DateTime.Now);
        if (existingTokens.Any()) return StatusCodes.Status429TooManyRequests;
        using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
        Token resetToken = new Token
        {
            UserId = user.UserId,
            TokenTypeId = 2,
            TokenExpiry = DateTime.Now.AddMinutes(15),
            TokenValue = SecurityService.GenerateSeed(preferMail ? 32 : 16)
        };
        await _context.Set<Token>().AddAsync(resetToken);
        await _context.SaveChangesAsync();

        Message message = new Message(_configuration, user)
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
                await transaction.RollbackAsync();
            else 
                await transaction.CommitAsync();
            return (int)response.StatusCode;
        }
        else if (!string.IsNullOrEmpty(user.Phone))
        {
            HttpResponseMessage response = await message.SendSmsAsync();
            if (!response.IsSuccessStatusCode)
                await transaction.RollbackAsync();
            else 
                await transaction.CommitAsync();
            return (int)response.StatusCode;
        }
        await transaction.RollbackAsync();
        return StatusCodes.Status400BadRequest;
    }

    public async Task<Tuple<int, string?>> EndResetPassword(string identifier, string code)
    {
        User? user = await GetByIdentifier(identifier);
        if (user == null) return new Tuple<int, string?>(StatusCodes.Status404NotFound, null);
        Token? token = await _context.Set<Token>()
            .FirstOrDefaultAsync(t =>
                t.UserId == user.UserId &&
                t.TokenTypeId == 2 &&
                t.TokenValue == code &&
                t.TokenExpiry > DateTime.Now);
        if (token == null) return new Tuple<int, string?>(StatusCodes.Status400BadRequest, null);
        string newPassword = SecurityService.GenerateSeed(12);
        if (!user.TrySetPassword(newPassword, true))
            return new Tuple<int, string?>(StatusCodes.Status500InternalServerError, null);
        _context.Set<Token>().Remove(token);
        await _context.SaveChangesAsync();
        return new Tuple<int, string?>(StatusCodes.Status200OK, newPassword);
    }

    public async Task<object?> ExportUserDataAsync(int userId)
    {
        QueryOptions<User> userOptions = new QueryOptions<User>()
            .Before("NotificationSettings.NotificationType",
                "Limits.LimitType", "Bans.BanType")
            .Before(u => u.Favorites);
        User? user = await GetByIdAsyncNew(userId, userOptions);
        if (user == null) return null;

        dynamic exportUser = new ExpandoObject();
        var exportUserDict = (IDictionary<string, object>)exportUser;
        foreach (var prop in user.GetType().GetProperties())
        {
            var propertyType = prop.PropertyType;
            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(propertyType) && propertyType != typeof(string))
                continue; // Skip navigation properties (IEnumerable, ICollection, complex types)
            if (propertyType.Namespace?.StartsWith(typeof(User).Namespace) ?? false)
                continue; // Skip custom entities / navigation properties (EF models)

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
        exportUser.RandomTransactions = randTrans.Select(rt => new {
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