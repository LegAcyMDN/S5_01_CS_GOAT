using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_usernotification_unf")]
    [Index(nameof(IsRead))]
    public partial class UserNotification : Notification
    {
        [Required]
        [Column("usr_id")]
        public int UserId { get; set; }

        [Required]
        [Column("unf_isread")]
        public bool IsRead { get; set; } = false;

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.UserNotifications))]
        public virtual User User { get; set; } = null!;
    }
}