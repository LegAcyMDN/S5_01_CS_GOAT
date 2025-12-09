using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_wear_wer")]
    [Index(nameof(Uuid))]
    public partial class Wear
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("wer_id")]
        public int WearId { get; set; }

        [Required]
        [Column("wrt_id")]
        public int WearTypeId { get; set; }

        [Required]
        [Column("wer_wearfloat")]
        [Range(0.0, 1.0)]
        public float WearFloat { get; set; }

        [Required]
        [StringLength(100)]
        [Column("wer_uuid")]
        public string Uuid { get; set; } = null!;

        [Required]
        [Column("skn_id")]
        public int SkinId { get; set; }

        [ForeignKey(nameof(WearTypeId))]
        [InverseProperty(nameof(WearType.Wears))]
        public virtual WearType WearType { get; set; } = null!;

        [ForeignKey(nameof(SkinId))]
        [InverseProperty(nameof(Skin.Wears))]
        public virtual Skin Skin { get; set; } = null!;

        [InverseProperty(nameof(InventoryItem.Wear))]
        public virtual ICollection<InventoryItem> InventoryItems { get; set; } = null!;
    }
}