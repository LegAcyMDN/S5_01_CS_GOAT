using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_j_wearclass_wrc")]
    public class WearClass
    {
        [Key]
        [Column("skn_id")]
        public int SkinId { get; set; }

        [Key]
        [Column("wrt_id")]
        public int WearTypeId { get; set; }

        [ForeignKey(nameof(SkinId))]
        [InverseProperty(nameof(Skin.WearClasses))]
        public virtual Skin Skin { get; set; } = null!;

        [ForeignKey(nameof(WearTypeId))]
        [InverseProperty(nameof(WearType.WearClasses))]
        public virtual WearType WearType { get; set; } = null!;

        [InverseProperty(nameof(Wear.WearClass))]
        public virtual ICollection<Wear> Wears { get; set; } = [];

        [InverseProperty(nameof(PriceHistory.WearClass))]
        public virtual ICollection<PriceHistory> PriceHistories { get; set; } = [];
    }
}
