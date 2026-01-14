using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Models.Repository
{
    /// <summary>
    /// Specifies the method of sending verification codes
    /// </summary>
    public enum SendMethod
    {
        /// <summary>Send via email</summary>
        Mail,
        /// <summary>Send via SMS</summary>
        Sms
    }

    /// <summary>
    /// Manages sending and verification of contact information (email/SMS) verification codes
    /// </summary>
    public interface ISendingRepository
    {
        /// <summary>
        /// Creates and sends a new verification code using the specified method
        /// </summary>
        /// <param name="method">The send method (Mail or SMS)</param>
        /// <param name="user">The user to send the code to</param>
        /// <returns>HTTP status code (201 for success, 400 for missing contact info, 409 for already verified, 429 for too many requests)</returns>
        Task<int> NewCodeAsync(SendMethod method, User user)
        {
            switch (method)
            {
                case SendMethod.Mail: return NewCodeMailAsync(user);
                case SendMethod.Sms: return NewCodeSmsAsync(user);
                default: throw new NotSupportedException($"Send method {method} is not supported.");
            }
        }
        Task<int> NewCodeMailAsync(User user);
        
        /// <summary>
        /// Creates and sends a new SMS verification code
        /// </summary>
        /// <param name="user">The user to send the code to</param>
        /// <returns>HTTP status code (201 for success, 400 for missing phone, 409 for already verified, 429 for too many requests)</returns>
        Task<int> NewCodeSmsAsync(User user);

        /// <summary>
        /// Verifies a code sent through the specified method
        /// </summary>
        /// <param name="method">The send method (Mail or SMS)</param>
        /// <param name="user">The user verifying the code</param>
        /// <param name="code">The verification code to verify</param>
        /// <returns>HTTP status code (200 for success, 400 for missing contact info, 404 for code not found, 409 for already verified, 410 for code expired)</returns>
        Task<int> VerifyAsync(SendMethod method, User user, string code)
        {
            switch (method)
            {
                case SendMethod.Mail: return VerifyMailAsync(user, code);
                case SendMethod.Sms: return VerifySmsAsync(user, code);
                default: throw new NotSupportedException($"Send method {method} is not supported.");
            }
        }
        Task<int> VerifyMailAsync(User user, string code);
        
        /// <summary>
        /// Verifies an SMS verification code
        /// </summary>
        /// <param name="user">The user verifying the code</param>
        /// <param name="code">The SMS verification code</param>
        /// <returns>HTTP status code (200 for success, 400 for missing phone, 404 for code not found, 409 for already verified, 410 for code expired)</returns>
        Task<int> VerifySmsAsync(User user, string code);
    }
}
