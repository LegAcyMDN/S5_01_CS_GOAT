using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_notification_ntf")]
    [Index(nameof(NotificationDate))]
    public abstract class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ntf_id")]
        public int NotificationId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("ntf_notificationsummary")]
        public string NotificationSummary { get; set; } = null!;

        [Required]
        [StringLength(1000)]
        [Column("ntf_notificationcontent")]
        public string NotificationContent { get; set; } = null!;

        [Required]
        [Column("ntf_notificationdate")]
        public DateTime NotificationDate { get; set; } = DateTime.Now;

        [Required]
        [Column("ntt_id")]
        public int NotificationTypeId { get; set; }

        [ForeignKey(nameof(NotificationTypeId))]
        [InverseProperty(nameof(NotificationType.Notifications))]
        public virtual NotificationType NotificationType { get; set; } = null!;

        [InverseProperty(nameof(Transaction.Notification))]
        public virtual ICollection<Transaction> Transactions { get; set; } = null!;
    }
}