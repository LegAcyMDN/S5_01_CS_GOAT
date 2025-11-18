using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_skin_skn")]
    public class Skin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("skn_id")]
        public int SkinId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("skn_skinname")]
        public string SkinName { get; set; } = null!;

        [Required]
        [Column("skn_paintindex")]
        public int PaintIndex { get; set; }

        [Required]
        [Column("itm_id")]
        public int ItemId { get; set; }

        [Required]
        [Column("rar_id")]
        public int RarityId { get; set; }

        [ForeignKey(nameof(ItemId))]
        [InverseProperty(nameof(Item.Skins))]
        public virtual Item Item { get; set; } = null!;

        [ForeignKey(nameof(RarityId))]
        [InverseProperty(nameof(Rarity.Skins))]
        public virtual Rarity Rarity { get; set; } = null!;

        [InverseProperty(nameof(CaseContent.Skin))]
        public virtual ICollection<CaseContent> CaseContents { get; set; } = null!;

        [InverseProperty(nameof(Wear.Skin))]
        public virtual ICollection<Wear> Wears { get; set; } = null!;
    }
}