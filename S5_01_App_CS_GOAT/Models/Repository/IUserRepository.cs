using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.DTO.Helpers;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Models.Repository;

public interface IUserRepository : IDataRepository<User, int>
{
    Task UpdateUserDetails(User existing, UpdateUserDTO userDTO);

    Task<User> CreateUser(CreateUserDTO newAccount);

    Task InitializeUser(User user);

    Task<AuthDTO> Auth(User user, IConfiguration config, int? remember = null);

    Task<User?> Login(LoginDTO loginDTO);

    Task<User?> Recall(RememberDTO rememberDTO);

    Task<User?> GetByIdentifier(string identifier);
    Task<User?> GetByLogin(string login);
    Task<User?> GetByEmail(string email);
    Task<User?> GetByPhone(string phone);

}