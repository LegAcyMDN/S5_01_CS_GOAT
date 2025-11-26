namespace S5_01_App_CS_GOAT.DTO
{
    public class NotificationDTO
    {
        public int NotificationId { get; set; }

        public string NotificationSummary { get; set; } = null!;

        public string NotificationContent { get; set; } = null!;
   
        public DateTime NotificationDate { get; set; }

        public string NotificationTypeName { get; set; } = null!;

    }
}
