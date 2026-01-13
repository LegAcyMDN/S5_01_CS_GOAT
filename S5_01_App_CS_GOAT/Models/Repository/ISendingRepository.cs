using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Models.Repository
{
    public enum SendMethod
    {
        Mail,
        Sms
    }

    public interface ISendingRepository
    {
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
        Task<int> NewCodeSmsAsync(User user);

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
        Task<int> VerifySmsAsync(User user, string code);
    }
}
