using Shared.DTO;
using Shared.DTO.Helpers;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Models.Repository;

public interface IUserRepository : IDataRepository<User, int>
{
    Task UpdateUserDetails(User existing, UpdateUserDTO userDTO);

    Task<User> CreateUser(CreateUserDTO newAccount);

    Task InitializeUser(User user);

    Task<AuthDTO> Auth(User user, IConfiguration config, int? remember = null);

    Task<User?> Login(LoginDTO loginDTO);

    Task<User?> Recall(TokenDTO rememberDTO);

    Task<User?> GetByIdentifier(string identifier);
    Task<User?> GetByLogin(string login);
    Task<User?> GetByEmail(string email);
    Task<User?> GetByPhone(string phone);

    Task<int> StartResetPassword(string identifier, string url, bool preferMail = true);
    Task<Tuple<int, string?>> EndResetPassword(string identifier, string code);

    Task<User?> GetBySteamIdAsync(string steamId);
    Task<User> AuthenticateWithSteam(SteamAuthDTO steamAuthDTO);

    Task<object?> ExportUserDataAsync(int userId);
}