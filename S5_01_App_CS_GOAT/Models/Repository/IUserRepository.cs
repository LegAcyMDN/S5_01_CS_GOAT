using Shared.DTO;
using Shared.DTO.Helpers;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Models.Repository;

/// <summary>
/// Manages user accounts, authentication, and profile management
/// </summary>
public interface IUserRepository : IDataRepository<User, int>
{
    /// <summary>
    /// Updates user profile details like display name, email, phone, and two-factor authentication settings
    /// </summary>
    /// <param name="existing">The user entity to update</param>
    /// <param name="userDTO">DTO containing updated user information</param>
    Task UpdateUserDetails(User existing, UpdateUserDTO userDTO);

    /// <summary>
    /// Creates a new user account with the provided account details
    /// </summary>
    /// <param name="newAccount">DTO containing new account creation data</param>
    /// <returns>The newly created User entity</returns>
    Task<User> CreateUser(CreateUserDTO newAccount);

    /// <summary>
    /// Initializes a new user account (e.g., sets up initial values, nonce, seed)
    /// </summary>
    /// <param name="user">The user to initialize</param>
    Task InitializeUser(User user);

    /// <summary>
    /// Authenticates a user and generates authorization tokens/claims
    /// </summary>
    /// <param name="user">The user to authenticate</param>
    /// <param name="config">Application configuration containing JWT settings</param>
    /// <param name="remember">Optional number of days to remember the login (for "Remember Me" functionality)</param>
    /// <returns>Authentication details including access token</returns>
    Task<AuthDTO> Auth(User user, IConfiguration config, int? remember = null);

    /// <summary>
    /// Authenticates a user with login credentials (username/email and password)
    /// </summary>
    /// <param name="loginDTO">DTO containing login credentials</param>
    /// <returns>The authenticated User if login successful; otherwise null</returns>
    Task<User?> Login(LoginDTO loginDTO);

    /// <summary>
    /// Recalls a user session from a remember token
    /// </summary>
    /// <param name="rememberDTO">DTO containing the remember token</param>
    /// <returns>The recalled User if token is valid; otherwise null</returns>
    Task<User?> Recall(TokenDTO rememberDTO);

    /// <summary>
    /// Gets a user by username, email, or phone identifier
    /// </summary>
    /// <param name="identifier">Username, email, or phone to search for</param>
    /// <returns>The User if found; otherwise null</returns>
    Task<User?> GetByIdentifier(string identifier);
    
    /// <summary>
    /// Gets a user by their login/username
    /// </summary>
    /// <param name="login">The user's login/username</param>
    /// <returns>The User if found; otherwise null</returns>
    Task<User?> GetByLogin(string login);
    
    /// <summary>
    /// Gets a user by their email address
    /// </summary>
    /// <param name="email">The user's email address</param>
    /// <returns>The User if found; otherwise null</returns>
    Task<User?> GetByEmail(string email);
    
    /// <summary>
    /// Gets a user by their phone number
    /// </summary>
    /// <param name="phone">The user's phone number</param>
    /// <returns>The User if found; otherwise null</returns>
    Task<User?> GetByPhone(string phone);

    /// <summary>
    /// Initiates a password reset by sending a code to the user's email or phone
    /// </summary>
    /// <param name="identifier">Username, email, or phone to identify the user</param>
    /// <param name="url">URL to include in the reset email (e.g., frontend reset link)</param>
    /// <param name="preferMail">If true, send code via email; otherwise via SMS</param>
    /// <returns>HTTP status code indicating result</returns>
    Task<int> StartResetPassword(string identifier, string url, bool preferMail = true);
    
    /// <summary>
    /// Completes a password reset by validating the code and updating the password
    /// </summary>
    /// <param name="identifier">Username, email, or phone to identify the user</param>
    /// <param name="code">The reset code provided by the user</param>
    /// <returns>Tuple of (HTTP status code, optional new temporary password if applicable)</returns>
    Task<Tuple<int, string?>> EndResetPassword(string identifier, string code);

    /// <summary>
    /// Gets a user by their Steam ID (for Steam authentication)
    /// </summary>
    /// <param name="steamId">The Steam ID to search for</param>
    /// <returns>The User if found; otherwise null</returns>
    Task<User?> GetBySteamIdAsync(string steamId);
    
    /// <summary>
    /// Authenticates or creates a user via Steam authentication
    /// </summary>
    /// <param name="steamAuthDTO">DTO containing Steam authentication data</param>
    /// <returns>The authenticated or newly created User</returns>
    Task<User> AuthenticateWithSteam(SteamAuthDTO steamAuthDTO);

    /// <summary>
    /// Exports all user data in a format suitable for GDPR data export requests
    /// </summary>
    /// <param name="userId">The ID of the user to export data for</param>
    /// <returns>Object containing all user data; null if user not found</returns>
    Task<object?> ExportUserDataAsync(int userId);
}