using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.DTO.Helpers;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Models.Repository;

public interface IUserRepository : IDataRepository<User, int>
{
    Task UpdateUserDetails(User existing, UserDetailDTO userDTO);

    Task<User> CreateUser(NewAccountDTO newAccount);
}