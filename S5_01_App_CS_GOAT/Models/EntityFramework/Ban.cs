using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_j_ban_ban")]
    [Index(nameof(BanDate))]
    public class Ban
    {
        [Key]
        [Column("usr_id", Order = 1)]
        public int UserId { get; set; }

        [Key]
        [Column("bnt_id", Order = 2)]
        public int BanTypeId { get; set; }

        [Required]
        [Column("ban_reason")]
        [StringLength(255)]
        public string BanReason { get; set; } = null!;

        [Required]
        [Column("ban_bandate")]
        public DateTime BanDate { get; set; } = DateTime.Now;

        [Required]
        [Column("ban_banduration")]
        [Range(1, int.MaxValue)]
        public int BanDuration { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.Bans))]
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(BanTypeId))]
        [InverseProperty(nameof(BanType.Bans))]
        public virtual BanType BanType { get; set; } = null!;
    }
}