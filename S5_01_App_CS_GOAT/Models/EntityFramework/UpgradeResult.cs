using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_j_upgraderesult_upg")]
    public class UpgradeResult
    {
        [Key]
        [Column("usr_id", Order = 1)]
        public int UserId { get; set; }

        [Key]
        [Column("wer_id", Order = 2)]
        public int WearId { get; set; }

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

        [ForeignKey($"{nameof(UserId)},{nameof(WearId)}")]
        [InverseProperty(nameof(InventoryItem.UpgradeResults))]
        public virtual InventoryItem InventoryItem { get; set; } = null!;

        [InverseProperty(nameof(RandomTransaction.UpgradeResult))]
        public virtual ICollection<RandomTransaction> RandomTransactions { get; set; } = null!;

        [InverseProperty(nameof(FairRandom.UpgradeResult))]
        public virtual ICollection<FairRandom> FairRandoms { get; set; } = null!;
    }
}