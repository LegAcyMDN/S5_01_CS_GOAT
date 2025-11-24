using Microsoft.IdentityModel.Tokens;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace S5_01_App_CS_GOAT.Services
{
    public struct AuthResult
    {
        public int? AuthUserId { get; set; }
        public bool IsAdmin { get; set; } = false;
        public bool IsAuthenticated => AuthUserId != null;

        public AuthResult(int? authUserId, bool isAdmin = false)
        {
            AuthUserId = authUserId;
            IsAdmin = isAdmin;
        }

        public bool IsAllowed(IUserDependant obj)
        {
            if (obj.DependantUserId == null) return true;
            if (!IsAuthenticated) return false;
            if (IsAdmin) return true;
            return obj.DependantUserId == AuthUserId;
        }
    }

    public abstract class JwtService
    {
        /// <summary>
        /// Generates the list of claims to be included in the JWT token
        /// </summary>
        /// <param name="user">The user using these claims</param>
        /// <returns>The list of claims</returns>
        public static List<Claim> GetClaims(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                 // Standard JWT claims
                 new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                 // Custom claims
                 new Claim(nameof(User.UserId), user.UserId.ToString()),
                 new Claim(nameof(User.Login), user.Login.ToString()),
                 new Claim(nameof(User.IsAdmin), user.IsAdmin.ToString())
            };
            return claims;
        }

        /// <summary>
        /// Generates a JWT token for the specified user
        /// </summary>
        /// <param name="user">The user using the JWT token</param>
        /// <returns>The serialized encrypted JWT token</returns>
        public static string GenerateJwtToken(User user)
        {
            string? secret = Environment.GetEnvironmentVariable("JWT_SECRET");
            if (secret == null) throw new Exception("JWT_SECRET environment variable is not set");
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = GetClaims(user);
            JwtSecurityToken token = new JwtSecurityToken
            (
                issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
                audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Checks if the current user is authorized to access the given IUserDependant ressource
        /// </summary>
        /// <param name="obj">The object to check for user ownership</param>
        /// <returns>AuthResult containing authentication and authorization status with user ID</returns>
        public static AuthResult Authorized(IUserDependant? obj)
        {
            // Bypass JWT authentification if the JWT_BYPASS environment variable is set (for testing)
            // Format: "@123" to spoof admin user with ID 123, "123" to spoof normal user with ID 123
            string? bypassAuth = Environment.GetEnvironmentVariable("JWT_BYPASS");
            if (!string.IsNullOrEmpty(bypassAuth))
            {
                bool spoofIsAdmin = bypassAuth.StartsWith("@");
                int? spoofUserId = int.Parse(bypassAuth.TrimStart('@'));
                return new AuthResult(spoofUserId, spoofIsAdmin);
            }

            // Find current context JWT authentification if present
            ClaimsIdentity ? identity = Thread.CurrentPrincipal?.Identity as ClaimsIdentity;
            if (identity == null) return new AuthResult
                { AuthUserId = null};

            // Get the inserted UserID
            Claim? userIdClaim = identity.FindFirst(nameof(User.UserId));
            if (userIdClaim == null) return new AuthResult
                { AuthUserId = null};

            // Get the inserted IsAdmin
            Claim? isAdminClaim = identity.FindFirst(nameof(User.IsAdmin));
            if (isAdminClaim == null) return new AuthResult
                { AuthUserId = null};

            // Get the user information
            bool isAdmin = bool.Parse(isAdminClaim.Value);
            int userId = int.Parse(userIdClaim.Value);
            return new AuthResult(userId, isAdmin);
        }

        public static IEnumerable<T> FilterByAuthorizedUser<T>(IEnumerable<T> collection) where T : IUserDependant
        {
            AuthResult auth = Authorized(null);
            return collection.Where(item => auth.IsAllowed(item));
        }
    }
}
