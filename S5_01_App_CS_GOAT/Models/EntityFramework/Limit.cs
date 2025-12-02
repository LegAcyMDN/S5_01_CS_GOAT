using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_j_limit_lim")]
    public partial class Limit
    {
        [Key]
        [Column("usr_id")]
        public int UserId { get; set; }

        [Key]
        [Column("lmt_id")]
        public int LimitTypeId { get; set; }

        [Column("lim_limitamount")]
        [Range(0.0, double.MaxValue)]
        public double? LimitAmount { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.Limits))]
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(LimitTypeId))]
        [InverseProperty(nameof(LimitType.Limits))]
        public virtual LimitType LimitType { get; set; } = null!;
    }
}