using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_weartype_wrt")]
    [Index(nameof(WearTypeName), IsUnique = true)]
    public partial class WearType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("wrt_id")]
        public int WearTypeId { get; set; }

        [Required]
        [Column("wrt_weartypename")]
        [StringLength(100)]
        public string WearTypeName { get; set; } = null!;

        [InverseProperty(nameof(PriceHistory.WearType))]
        public virtual ICollection<PriceHistory> PriceHistories { get; set; } = null!;

        [InverseProperty(nameof(Wear.WearType))]
        public virtual ICollection<Wear> Wears { get; set; } = null!;
    }
}