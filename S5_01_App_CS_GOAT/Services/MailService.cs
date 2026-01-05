using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Services
{
    public class Email
    {
        public readonly IConfiguration _configuration;
        public readonly User? user;
        public readonly string fromAddress;
        public readonly string? fromName;
        public readonly string toAddress;
        public readonly string? toName;
        public readonly string subject;
        public readonly string content;

        public Email(IConfiguration configuration, string toAddress, string? toName, string subject, string content)
        {
            _configuration = configuration;
            this.fromAddress = configuration["Mail:FromAddress"] ??
                throw new ArgumentException("Mail FromAddress is not configured");
            this.fromName = configuration["Mail:FromName"];
            this.toAddress = toAddress;
            this.toName = toName;
            this.subject = subject;
            this.content = content;
        }

        public Email(IConfiguration configuration, User user, string subject, string content)
            : this(configuration, user.Email ?? throw new ArgumentException("User email is null"), user.DisplayName, subject, content)
        {
            this.user = user;
        }

        public async Task<HttpResponseMessage> SendAsync()
        {
            return await MailService.SendDebugEmailAsync(_configuration, this);
        }
    }

    public class MailService
    {
        public static async Task<HttpResponseMessage> SendDebugEmailAsync(IConfiguration configuration, Email email)
        {
            string url = configuration["Mail:Url"]
                ?? throw new ArgumentException("MailTrap URL is not configured");
            string token = configuration["Mail:Token"]
                ?? throw new ArgumentException("MailTrap Token is not configured");

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var payload = new
            {
                from = new
                {
                    email = email.fromAddress,
                    name = email.fromName
                },
                to = new[]
                {
                    new
                    {
                        email = configuration["Mail:ReplaceTo"] ?? email.toAddress,
                        name = email.toName
                    }
                },
                subject = email.subject,
                html = email.content
            };

            return await client.PostAsJsonAsync(url, payload);
        }
    }

}
