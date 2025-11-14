using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_j_notificationsetting_nts")]
    public class NotificationSetting
    {
        [Key]
        [Column("usr_id", Order = 1)]
        public int UserId { get; set; }

        [Key]
        [Column("ntt_id", Order = 2)]
        public int NotificationTypeId { get; set; }

        [Required]
        [Column("nts_onsite")]
        public bool OnSite { get; set; }

        [Required]
        [Column("nts_byemail")]
        public bool ByEmail { get; set; }

        [Required]
        [Column("nts_byphone")]
        public bool ByPhone { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.NotificationSettings))]
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(NotificationTypeId))]
        [InverseProperty(nameof(NotificationType.NotificationSettings))]
        public virtual NotificationType NotificationType { get; set; } = null!;
    }
}