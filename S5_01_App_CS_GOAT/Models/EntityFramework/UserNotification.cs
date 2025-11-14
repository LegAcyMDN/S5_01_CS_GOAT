using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_usernotification_unf")]
    public class UserNotification : Notification
    {
        [Required]
        [Column("usr_id")]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.UserNotifications))]
        public virtual User User { get; set; } = null!;
    }
}