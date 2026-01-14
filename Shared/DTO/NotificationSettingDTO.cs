namespace Shared.DTO
{
    public class NotificationSettingDTO
    {
        public bool OnSite { get; set; }

        public bool ByEmail { get; set; }

        public bool ByPhone { get; set; }

        public string NotificationTypeName { get; set; } = null!;
    }
}
