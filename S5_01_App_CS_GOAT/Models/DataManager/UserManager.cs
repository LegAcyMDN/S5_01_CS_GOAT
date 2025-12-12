using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Partials;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using S5_01_App_CS_GOAT.DTO.Helpers;
using Microsoft.EntityFrameworkCore;

namespace S5_01_App_CS_GOAT.Models.DataManager;

public class UserManager : CrudRepository<User, int>, IUserRepository
{
    protected readonly CSGOATDbContext _context;
    public UserManager(CSGOATDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task UpdateUserDetails(User existing, UpdateUserDTO userDTO)
    {
        if (userDTO.Email == null && userDTO.Phone == null)
            throw new InvalidOperationException("At least one contact method (email or phone) must be provided.");

        _context.Set<User>().Attach(existing);

        bool needPasswordCheck = false;

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

    public async Task<User?> Recall(RememberDTO rememberDTO)
    {
        Token? token = await _context.Set<Token>().FindAsync(rememberDTO.TokenId);
        if (
            token == null
            || token.TokenValue != rememberDTO.Token
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
            JwtToken = jwtToken,
            RememberToken = rememberToken
        };
        if (rememberToken != null)
        {
            await _context.Set<Token>().AddAsync(rememberToken);
            await _context.SaveChangesAsync();
        }
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
}