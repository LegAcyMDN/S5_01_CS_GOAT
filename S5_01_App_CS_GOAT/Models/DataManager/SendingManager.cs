using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class SendingManager: ISendingRepository
    {
        protected readonly CSGOATDbContext _context;
        protected readonly IUserRepository _userRepository;
        protected readonly IConfiguration _configuration;

        public SendingManager(
            CSGOATDbContext context,
            IUserRepository userRepository,
            IConfiguration configuration)
        {
            _context = context;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        private async Task<Token> NewToken(int userId, int tokenTypedId,
            TimeSpan? duration = null, string? code = null)
        {
            duration ??= TimeSpan.FromMinutes(15);
            code ??= SecurityService.GenerateSeed(6);
            Token token = new Token
            {
                UserId = userId,
                TokenTypeId = tokenTypedId,
                TokenExpiry = DateTime.Now.Add(duration.Value),
                TokenValue = code
            };
            _context.Set<Token>().Add(token);
            await _context.SaveChangesAsync();
            return token;
        }

        private Message NewMessage(User user, string token)
        {
            Message message = new Message(_configuration, user)
            {
                Text = $"Your CS:GOAT verification code is: {token}",
                Subject = "CS:GOAT Verification Code",
                Html = $"<p>Your CS:GOAT verification code is: <strong>{token}</strong></p>"
            };
            return message;
        }

        /// <summary>
        /// Generic method to create and send a verification code
        /// </summary>
        private async Task<int> NewCodeAsync(
            User user,
            int tokenTypeId,
            string? contactInfo,
            DateTime? verifiedOn,
            Func<Message, Task<HttpResponseMessage>> sendMethod)
        {
            if (string.IsNullOrWhiteSpace(contactInfo))
                return StatusCodes.Status400BadRequest;
            if (verifiedOn != null)
                return StatusCodes.Status409Conflict;

            // Check for existing unexpired tokens
            IEnumerable<Token> tokens = _context.Set<Token>()
                .Where(t => t.UserId == user.UserId && t.TokenTypeId == tokenTypeId);
            // If any unexpired token exists, do not create a new one
            if (tokens.Any(t => t.TokenExpiry > DateTime.Now))
                return StatusCodes.Status429TooManyRequests;

            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            // Otherwise, remove all previous tokens
            _context.Set<Token>().RemoveRange(tokens);
            Token token = await NewToken(user.UserId, tokenTypeId);
            
            // Create the message and send it
            Message message = NewMessage(user, token.TokenValue);
            HttpResponseMessage response = await sendMethod(message);
            if (!response.IsSuccessStatusCode)
            {
                await transaction.RollbackAsync();
                return StatusCodes.Status500InternalServerError;
            }
            await transaction.CommitAsync();
            return StatusCodes.Status201Created;
        }

        /// <summary>
        /// Generic method to verify a code
        /// </summary>
        private async Task<int> VerifyCodeAsync(
            User user,
            string code,
            int tokenTypeId,
            string? contactInfo,
            DateTime? verifiedOn,
            Action<User> setVerifiedOn)
        {
            if (string.IsNullOrWhiteSpace(contactInfo))
                return StatusCodes.Status400BadRequest;
            if (verifiedOn != null)
                return StatusCodes.Status409Conflict;

            // Find the token
            Token? token = _context.Set<Token>()
                .FirstOrDefault(t =>
                    t.UserId == user.UserId &&
                    t.TokenTypeId == tokenTypeId &&
                    t.TokenValue == code);
            if (token == null) return StatusCodes.Status404NotFound;

            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            int statusCode;
            // Check token expiry
            if (token.TokenExpiry < DateTime.Now)
            {
                statusCode = StatusCodes.Status410Gone;
            }
            else
            {
                setVerifiedOn(user);
                _context.Set<User>().Update(user);
                statusCode = StatusCodes.Status200OK;
            }
            // Remove the token
            _context.Set<Token>().Remove(token);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return statusCode;
        }

        public async Task<int> NewCodeMailAsync(User user)
        {
            return await NewCodeAsync(
                user,
                tokenTypeId: 3,
                contactInfo: user.Email,
                verifiedOn: user.EmailVerifiedOn,
                sendMethod: message => message.SendMailAsync()
            );
        }

        public async Task<int> NewCodeSmsAsync(User user)
        {
            return await NewCodeAsync(
                user,
                tokenTypeId: 4,
                contactInfo: user.Phone,
                verifiedOn: user.PhoneVerifiedOn,
                sendMethod: message => message.SendSmsAsync()
            );
        }

        public async Task<int> VerifyMailAsync(User user, string code)
        {
            return await VerifyCodeAsync(
                user,
                code,
                tokenTypeId: 3,
                contactInfo: user.Email,
                verifiedOn: user.EmailVerifiedOn,
                setVerifiedOn: u => u.EmailVerifiedOn = DateTime.Now
            );
        }

        public async Task<int> VerifySmsAsync(User user, string code)
        {
            return await VerifyCodeAsync(
                user,
                code,
                tokenTypeId: 4,
                contactInfo: user.Phone,
                verifiedOn: user.PhoneVerifiedOn,
                setVerifiedOn: u => u.PhoneVerifiedOn = DateTime.Now
            );
        }
    }
}
