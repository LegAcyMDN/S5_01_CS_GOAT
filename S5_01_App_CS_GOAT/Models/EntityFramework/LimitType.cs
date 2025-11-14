using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_limittype_lmt")]
    public class LimitType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("lmt_id")]
        public int LimitTypeId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("lmt_limittypename")]
        public string LimitTypeName { get; set; } = null!;

        [Required]
        [Column("lmt_duration")]
        public int Duration { get; set; }

        [Required]
        [StringLength(50)]
        [Column("lmt_durationname")]
        public string DurationName { get; set; } = null!;

        [InverseProperty(nameof(Limit.LimitType))]
        public virtual ICollection<Limit> Limits { get; set; }
    }
}