using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_globalnotification_gnf")]
    [Index(nameof(IncludeVisitors))]
    public class GlobalNotification : Notification
    {
        [Required]
        [Column("gnf_includevisitors")]
        public bool IncludeVisitors { get; set; }
    }
}