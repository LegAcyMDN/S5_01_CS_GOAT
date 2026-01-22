using System.Net.Http.Headers;
using System.Reflection;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Services
{
    /// <summary>
    /// Base interface for message-related data including configuration, user, and message content
    /// </summary>
    public interface IMessage
    {
        IConfiguration? Configuration { get; set; }
        User? User { get; set; }
        string? Text { get; set; }
        string? Subject { get; set; }
        string? Html { get; set; }
    }

    /// <summary>
    /// Generic interface for messageable entities that can be sent via external services
    /// </summary>
    /// <typeparam name="T">The content type (Tuple&lt;string, string&gt; for email, string for SMS)</typeparam>
    public interface IMessageable<T> : IMessage
    {
        /// <summary>
        /// Determines the configuration section name based on the content type
        /// </summary>
        private static string ConfigString => typeof(T) == typeof(Tuple<string, string>) ? "Mail" : "Sms";

        /// <summary>Gets the API endpoint URL from configuration</summary>
        string? Url => Configuration?[ConfigString + ":Url"];
        /// <summary>Gets the authentication scheme (e.g., "Bearer", "Basic") from configuration</summary>
        string? Auth => Configuration?[ConfigString + ":Auth"];
        /// <summary>Gets the authentication token from configuration</summary>
        string? Token => Configuration?[ConfigString + ":Token"];
        /// <summary>Gets the content type (e.g., "application/json") from configuration</summary>
        string? Type => Configuration?[ConfigString + ":Type"];
        /// <summary>Gets the sender address/number from configuration</summary>
        string? From => Configuration?[ConfigString + ":From"];
        /// <summary>Gets the recipient address/number from the user (overridden by implementations)</summary>
        virtual string? ToUser => null;
        /// <summary>Gets the override recipient from configuration for testing</summary>
        string? ToOverride => Configuration?[ConfigString + ":To"];
        /// <summary>Gets the final recipient address/number (override takes precedence)</summary>
        string? To => ToOverride ?? ToUser;

        /// <summary>Gets whether the message has all required information to be sent</summary>
        bool CanSend { get; }
        /// <summary>Gets the formatted content ready to be sent</summary>
        T? Content { get; }
        /// <summary>Gets the payload object to be serialized and sent to the API</summary>
        object? Payload { get; }
    }

    /// <summary>
    /// Interface for email messaging with specific mail-related properties and validation
    /// </summary>
    public interface IMail : IMessageable<Tuple<string, string>>
    {
        /// <summary>Gets the origin/sender name for the email</summary>
        string? Origin { get; }
        /// <summary>Gets the recipient's display name</summary>
        string? Name => User?.DisplayName ?? User?.Login;
        /// <summary>Gets the recipient's email address from the user</summary>
        new string? ToUser => User?.Email;

        /// <summary>
        /// Validates that all required fields are present (From, To, and at least two of Text/Subject/Html)
        /// </summary>
        bool IMessageable<Tuple<string, string>>.CanSend => !string.IsNullOrWhiteSpace(From) // Requires FromAddress
                && !string.IsNullOrWhiteSpace(To) // Requires ToAddress
                                                  // Requires at least two of Text, Subject or Html
                && ((string.IsNullOrWhiteSpace(Text) ? 0 : 1) + (string.IsNullOrWhiteSpace(Subject) ? 0 : 1) + (string.IsNullOrWhiteSpace(Html) ? 0 : 1) >= 2);

        /// <summary>
        /// Creates a tuple containing the email subject and HTML body
        /// </summary>
        Tuple<string, string>? IMessageable<Tuple<string, string>>.Content => CanSend ?
                new Tuple<string, string>(
                    Subject ?? Text!,
                    Html ?? Text!
                )
                : null;

        /// <summary>
        /// Creates the JSON payload object for the email API request
        /// </summary>
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

    /// <summary>
    /// Interface for SMS messaging with specific SMS-related properties and validation
    /// </summary>
    public interface ISms : IMessageable<string>
    {
        /// <summary>Gets the recipient's phone number from the user</summary>
        new string? ToUser => User?.Phone;

        /// <summary>
        /// Validates that all required fields are present (From, To, and at least one of Text/Subject/Html)
        /// </summary>
        bool IMessageable<string>.CanSend => !string.IsNullOrWhiteSpace(From) // Requires FromNumber
                && !string.IsNullOrWhiteSpace(To) // Requires ToNumber
                                                  // Requires at least one of Text, Subject or Html
                && !string.IsNullOrWhiteSpace(Text + Subject + Html);

        /// <summary>
        /// Gets the SMS message text (prioritizes Text, then Subject, then Html)
        /// </summary>
        string? IMessageable<string>.Content => CanSend ?
                (Text ?? Subject ?? Html)!
                : null;

        /// <summary>
        /// Creates the payload object for the SMS API request
        /// </summary>
        object? IMessageable<string>.Payload => Content != null ? new
        {
            To,
            From,
            Body = Content
        } : null;
    }

    /// <summary>
    /// Concrete implementation of messaging service that supports both email and SMS sending
    /// </summary>
    public class Message : IMail, ISms
    {
        public IConfiguration? Configuration { get; set; }
        public User? User { get; set; }
        /// <summary>Gets the origin name for messages ("CS:GOAT")</summary>
        public string? Origin => "CS:GOAT";
        public string? Text { get; set; }
        public string? Subject { get; set; }
        public string? Html { get; set; }

        /// <summary>
        /// Initializes a new instance of the Message class
        /// </summary>
        /// <param name="configuration">The application configuration containing API settings</param>
        /// <param name="user">The user to send the message to</param>
        public Message(IConfiguration? configuration = null, User? user = null)
        {
            Configuration = configuration;
            User = user;
        }

        /// <summary>
        /// Encodes a payload object into the specified HttpContent type
        /// </summary>
        /// <typeparam name="T">The target HttpContent type (StringContent for JSON or FormUrlEncodedContent for form data)</typeparam>
        /// <param name="payload">The object to encode</param>
        /// <returns>The encoded HttpContent</returns>
        /// <exception cref="NotSupportedException">Thrown when the target type is not supported</exception>
        public T EncodePayload<T>(object payload) where T : HttpContent
        {
            switch (typeof(T))
            {
                case Type t when t == typeof(StringContent):
                    // Serialize as JSON
                    string jsonString = System.Text.Json.JsonSerializer.Serialize(payload);
                    return (T)(HttpContent)new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");
                case Type t when t == typeof(FormUrlEncodedContent):
                    // Serialize as form data
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

        /// <summary>
        /// Sends a message asynchronously via the configured API
        /// </summary>
        /// <typeparam name="T1">The messageable interface type (IMail or ISms)</typeparam>
        /// <typeparam name="T2">The content type</typeparam>
        /// <returns>The HTTP response from the API</returns>
        /// <exception cref="InvalidOperationException">Thrown when required message information is missing</exception>
        /// <exception cref="NotSupportedException">Thrown when the content type is not supported</exception>
        public async Task<HttpResponseMessage> SendAsync<T1, T2>() where T1 : IMessageable<T2>
        {
            var messageable = (T1)(IMessageable<T2>)this;
            if (messageable.Payload == null)
            {
                throw new InvalidOperationException("Message cannot be sent due to missing information.");
            }

            using var client = new HttpClient();
            // Set up authentication if configured
            if (messageable.Auth != null && messageable.Token != null)
            {
                string token = messageable.Token;
                // Base64 encode for Basic auth
                if (messageable.Auth == "Basic")
                {
                    token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(token));
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(messageable.Auth, token);
            }
            // Send the request based on content type
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

        /// <summary>
        /// Sends an email message asynchronously
        /// </summary>
        /// <returns>The HTTP response from the mail API</returns>
        public Task<HttpResponseMessage> SendMailAsync()
        {
            return SendAsync<IMail, Tuple<string, string>>();
        }

        /// <summary>
        /// Sends an SMS message asynchronously
        /// </summary>
        /// <returns>The HTTP response from the SMS API</returns>
        public Task<HttpResponseMessage> SendSmsAsync()
        {
            return SendAsync<ISms, string>();
        }
    }
}
