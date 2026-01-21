using System.Net.Http.Headers;
using System.Reflection;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Services
{
    public interface IMessage
    {
        IConfiguration? Configuration { get; set; }
        User? User { get; set; }
        string? Text { get; set; }
        string? Subject { get; set; }
        string? Html { get; set; }
    }

    public interface IMessageable<T> : IMessage
    {
        private static string ConfigString => typeof(T) == typeof(Tuple<string, string>) ? "Mail" : "Sms";

        string? Url => Configuration?[ConfigString + ":Url"];
        string? Auth => Configuration?[ConfigString + ":Auth"];
        string? Token => Configuration?[ConfigString + ":Token"];
        string? Type => Configuration?[ConfigString + ":Type"];
        string? From => Configuration?[ConfigString + ":From"];
        virtual string? ToUser => null;
        string? ToOverride => Configuration?[ConfigString + ":To"];
        string? To => ToOverride ?? ToUser;

        bool CanSend { get; }
        T? Content { get; }
        object? Payload { get; }
    }

    public interface IMail : IMessageable<Tuple<string, string>>
    {
        string? Origin { get; }
        string? Name => User?.DisplayName ?? User?.Login;
        new string? ToUser => User?.Email;

        bool IMessageable<Tuple<string, string>>.CanSend => !string.IsNullOrWhiteSpace(From) // Requires FromAddress
                && !string.IsNullOrWhiteSpace(To) // Requires ToAddress
                                                  // Requires at least two of Text, Subject or Html
                && ((string.IsNullOrWhiteSpace(Text) ? 0 : 1) + (string.IsNullOrWhiteSpace(Subject) ? 0 : 1) + (string.IsNullOrWhiteSpace(Html) ? 0 : 1) >= 2);

        Tuple<string, string>? IMessageable<Tuple<string, string>>.Content => CanSend ?
                new Tuple<string, string>(
                    Subject ?? Text!,
                    Html ?? Text!
                )
                : null;

        object? IMessageable<Tuple<string, string>>.Payload => Content != null ? new
        {
            from = new
            {
                email = From,
                name = Origin
            },
            to = new[]
                {
                    new
                    {
                        email = To,
                        name = Name
                    }
                },
            subject = Content!.Item1,
            html = Content!.Item2
        } : null;
    }

    public interface ISms : IMessageable<string>
    {
        new string? ToUser => User?.Phone;

        bool IMessageable<string>.CanSend => !string.IsNullOrWhiteSpace(From) // Requires FromNumber
                && !string.IsNullOrWhiteSpace(To) // Requires ToNumber
                                                  // Requires at least one of Text, Subject or Html
                && !string.IsNullOrWhiteSpace(Text + Subject + Html);

        string? IMessageable<string>.Content => CanSend ?
                (Text ?? Subject ?? Html)!
                : null;

        object? IMessageable<string>.Payload => Content != null ? new
        {
            To,
            From,
            Body = Content
        } : null;
    }

    public class Message : IMail, ISms
    {
        public IConfiguration? Configuration { get; set; }
        public User? User { get; set; }
        public string? Origin => "CS:GOAT";
        public string? Text { get; set; }
        public string? Subject { get; set; }
        public string? Html { get; set; }

        public Message(IConfiguration? configuration = null, User? user = null)
        {
            Configuration = configuration;
            User = user;
        }

        public T EncodePayload<T>(object payload) where T : HttpContent
        {
            switch (typeof(T))
            {
                case Type t when t == typeof(StringContent):
                    string jsonString = System.Text.Json.JsonSerializer.Serialize(payload);
                    return (T)(HttpContent)new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");
                case Type t when t == typeof(FormUrlEncodedContent):
                    var dict = new Dictionary<string, string>();
                    foreach (PropertyInfo prop in payload.GetType().GetProperties())
                    {
                        string value = prop.GetValue(payload)?.ToString() ?? "";
                        dict.Add(prop.Name, value);
                    }
                    return (T)(HttpContent)new FormUrlEncodedContent(dict);
                default:
                    throw new NotSupportedException($"Encoding to {typeof(T).Name} is not supported.");
            }
        }

        public async Task<HttpResponseMessage> SendAsync<T1, T2>() where T1 : IMessageable<T2>
        {
            var messageable = (T1)(IMessageable<T2>)this;
            if (messageable.Payload == null)
            {
                throw new InvalidOperationException("Message cannot be sent due to missing information.");
            }

            using var client = new HttpClient();
            if (messageable.Auth != null && messageable.Token != null)
            {
                string token = messageable.Token;
                if (messageable.Auth == "Basic")
                {
                    token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(token));
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(messageable.Auth, token);
            }
            switch (messageable.Type)
            {
                case "application/json":
                    StringContent jsonContent = EncodePayload<StringContent>(messageable.Payload);
                    return await client.PostAsync(messageable.Url!, jsonContent);
                case "application/x-www-form-urlencoded":
                    FormUrlEncodedContent formContent = EncodePayload<FormUrlEncodedContent>(messageable.Payload);
                    return await client.PostAsync(messageable.Url!, formContent);
                default:
                    throw new NotSupportedException($"Content type {messageable.Type} is not supported.");
            }
        }

        public Task<HttpResponseMessage> SendMailAsync()
        {
            return SendAsync<IMail, Tuple<string, string>>();
        }

        public Task<HttpResponseMessage> SendSmsAsync()
        {
            return SendAsync<ISms, string>();
        }
    }
}
