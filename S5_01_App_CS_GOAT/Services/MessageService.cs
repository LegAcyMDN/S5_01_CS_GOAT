using System.Linq.Expressions;
using System.Reflection;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using System.Net.Http.Headers;

namespace S5_01_App_CS_GOAT.Services
{
    public interface IMessage
    {
        public IConfiguration? Configuration { get; set; }
        public User? User { get; set; }
        public string? Text { get; set; }
        public string? Subject { get; set; }
        public string? Html { get; set; }
    }

    public interface IMessageable<T> : IMessage
    {
        private static string ConfigString => typeof(T) == typeof(Tuple<string, string>) ? "Mail" : "Sms";

        public string? Url => Configuration?[ConfigString + ":Url"];
        public string? Auth => Configuration?[ConfigString + ":Auth"];
        public string? Token => Configuration?[ConfigString + ":Token"];
        public string? Type => Configuration?[ConfigString + ":Type"];
        public string? From => Configuration?[ConfigString + ":From"];
        public virtual string? ToUser => null;
        public string? ToOverride => Configuration?[ConfigString + ":To"];
        public string? To => ToOverride ?? ToUser;

        public bool CanSend { get; }
        public T? Content { get; }
        public object? Payload { get; }
    }

    public interface IMail : IMessageable<Tuple<string, string>>
    {
        public string? Origin { get; }
        public string? Name => User?.DisplayName ?? User?.Login;
        public new string? ToUser => User?.Email;

        bool IMessageable<Tuple<string, string>>.CanSend => !string.IsNullOrWhiteSpace(From) // Requires FromAddress
                && !string.IsNullOrWhiteSpace(To) // Requires ToAddress
                // Requires at least two of Text, Subject or Html
                && ((string.IsNullOrWhiteSpace(Text) ? 0 : 1) + (string.IsNullOrWhiteSpace(Subject) ? 0 : 1) + (string.IsNullOrWhiteSpace(Html) ? 0 : 1) >= 2);

        Tuple<string, string>? IMessageable<Tuple<string, string>>.Content => this.CanSend ?
                new Tuple<string, string>(
                    Subject ?? Text!,
                    Html ?? Text!
                )
                : null;

        object? IMessageable<Tuple<string, string>>.Payload => this.Content != null ? new
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
            subject = this.Content!.Item1,
            html = this.Content!.Item2
        } : null;
    }

    public interface ISms : IMessageable<string>
    {
        public new string? ToUser => User?.Phone;

        bool IMessageable<string>.CanSend => !string.IsNullOrWhiteSpace(From) // Requires FromNumber
                && !string.IsNullOrWhiteSpace(To) // Requires ToNumber
                // Requires at least one of Text, Subject or Html
                && !string.IsNullOrWhiteSpace(Text + Subject + Html);

        string? IMessageable<string>.Content => this.CanSend ?
                (Text ?? Subject ?? Html)!
                : null;

        object? IMessageable<string>.Payload => this.Content != null ? new
        {
            To = To,
            From = From,
            Body = this.Content
        } : null;
    }

    public class Message : IMail, ISms
    {
        public IConfiguration? Configuration { get; set; }
        public User? User { get; set; }
        public string? Origin { get => "CS:GOAT";}
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
                        var value = prop.GetValue(payload)?.ToString() ?? "";
                        dict.Add(prop.Name, value);
                    }
                    return (T)(HttpContent)new FormUrlEncodedContent(dict);
                default:
                    throw new NotSupportedException($"Encoding to {typeof(T).Name} is not supported.");
            }
        }

        public async Task<HttpResponseMessage> SendAsync<T1, T2>() where T1: IMessageable<T2>
        {
            T1 messageable = (T1)(IMessageable<T2>)this;
            if (messageable.Payload == null)
                throw new InvalidOperationException("Message cannot be sent due to missing information.");
            using HttpClient client = new HttpClient();
            if (messageable.Auth != null && messageable.Token != null)
            {
                string token = messageable.Token;
                if (messageable.Auth == "Basic")
                    token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(token));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(messageable.Auth, token);
            }
            switch (messageable.Type)
            {
                case "application/json":
                    var jsonContent = EncodePayload<StringContent>(messageable.Payload);
                    return await client.PostAsync(messageable.Url!, jsonContent);
                case "application/x-www-form-urlencoded":
                    var formContent = EncodePayload<FormUrlEncodedContent>(messageable.Payload);
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
