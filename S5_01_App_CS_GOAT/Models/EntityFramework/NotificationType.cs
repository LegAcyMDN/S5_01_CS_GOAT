using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_notificationtype_ntt")]
    [Index(nameof(NotificationTypeName), IsUnique = true)]
    public class NotificationType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ntt_id")]
        public int NotificationTypeId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("ntt_notificationtypename")]
        public string NotificationTypeName { get; set; } = null!;

        [InverseProperty(nameof(NotificationSetting.NotificationType))]
        public virtual ICollection<NotificationSetting> NotificationSettings { get; set; } = null!;

        [InverseProperty(nameof(Notification.NotificationType))]
        public virtual ICollection<Notification> Notifications { get; set; } = null!;
    }
}