using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_j_upgraderesult_upg")]
    public partial class UpgradeResult
    {
        [Key]
        [Column("inv_id")]
        public int InventoryItemId { get; set; }

        [Column("tra_id")]
        public int TransactionId { get; set; }

        [Required]
        [Column("upg_floatstart")]
        [Range(0.0, 1.0)]
        public float FloatStart { get; set; }

        [Required]
        [Column("upg_floatend")]
        [Range(0.0, 1.0)]
        public float FloatEnd { get; set; }

        [Required]
        [Column("upg_probintact")]
        [Range(0.0, 1.0)]
        public double ProbIntact { get; set; }

        [Required]
        [Column("upg_probdegrade")]
        [Range(0.0, 1.0)]
        public double ProbDegrade { get; set; }

        [Required]
        [Column("upg_propdestroy")]
        [Range(0.0, 1.0)]
        public double PropDestroy { get; set; }

        [Required]
        [Column("upg_degradefuction")]
        [StringLength(100)]
        public string DegradeFunction { get; set; } = null!;

        [Required]
        [Column("frn_id")]
        public int FairRandomId { get; set; }

        [ForeignKey(nameof(InventoryItemId))]
        [InverseProperty(nameof(InventoryItem.UpgradeResults))]
        public virtual InventoryItem InventoryItem { get; set; } = null!;

        [ForeignKey(nameof(TransactionId))]
        [InverseProperty(nameof(RandomTransaction.UpgradeResults))]
        public virtual RandomTransaction RandomTransaction { get; set; } = null!;

        [ForeignKey(nameof(FairRandomId))]
        [InverseProperty(nameof(FairRandom.UpgradeResult))]
        public virtual FairRandom FairRandom { get; set; } = null!;
    }
}