using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Partials;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using S5_01_App_CS_GOAT.DTO.Helpers;

namespace S5_01_App_CS_GOAT.Models.DataManager;

public class UserManager : CrudRepository<User, int>, IUserRepository
{
    protected readonly CSGOATDbContext _context;
    public UserManager(CSGOATDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task UpdateUserDetails(User existing, UserDetailDTO userDTO)
    {
        if (userDTO.Email == null && userDTO.Phone == null)
            throw new InvalidOperationException("At least one contact method (email or phone) must be provided.");

        _context.Set<User>().Attach(existing);

        existing.DisplayName = userDTO.DisplayName;

        if (userDTO.Phone != existing.Phone)
        {
            existing.Phone = userDTO.Phone;
            existing.PhoneVerifiedOn = null;
        }

        if (userDTO.Email != existing.Email)
        {
            existing.Email = userDTO.Email;
            existing.EmailVerifiedOn = null;
        }

        if (userDTO.TwoFA == TwoFAmethod.None)
        {
            existing.TwoFaIsEmail = false;
            existing.TwoFaIsPhone = false;
        }
        else if (userDTO.TwoFA == TwoFAmethod.Email)
        {
            if (existing.EmailVerifiedOn == null)
                throw new InvalidOperationException("Cannot enable email 2FA without a verified email.");
            existing.TwoFaIsEmail = true;
            existing.TwoFaIsPhone = false;
        }
        else if (userDTO.TwoFA == TwoFAmethod.Phone)
        {
            if (existing.EmailVerifiedOn == null)
                throw new InvalidOperationException("Cannot enable phone 2FA without a verified phone.");
            existing.TwoFaIsEmail = false;
            existing.TwoFaIsPhone = true;
        }

        existing.Seed = string.IsNullOrEmpty(userDTO.Seed) ? SecurityService.GenerateRandomSeed() : userDTO.Seed;

        await _context.SaveChangesAsync();
    }

    public async Task<User> CreateUser(NewAccountDTO newAccount)
    {
        if (newAccount.Email == null && newAccount.Phone == null)
            throw new InvalidOperationException("At least one contact method (email or phone) must be provided.");

        User newUser = new User
        {
            Login = newAccount.Login,
            DisplayName = newAccount.DisplayName,
            Email = newAccount.Email,
            Phone = newAccount.Phone
        };

        await AddAsync(newUser);
        return newUser;
    }
}