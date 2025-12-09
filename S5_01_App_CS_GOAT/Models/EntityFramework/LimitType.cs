using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_limittype_lmt")]
    [Index(nameof(LimitTypeName), IsUnique = true)]
    public partial class LimitType
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
        [Range(1, int.MaxValue)]
        public int Duration { get; set; }

        [InverseProperty(nameof(Limit.LimitType))]
        public virtual ICollection<Limit> Limits { get; set; } = null!;
    }
}