using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_randomtransaction_rtr")]
    public class RandomTransaction : ItemTransaction
    {
        [Column("cas_id")]
        public int? CaseId { get; set; }

        [Column("frn_id")]
        public int FairRandomId { get; set; }

        [ForeignKey(nameof(FairRandomId))]
        [InverseProperty(nameof(FairRandom.RandomTransaction))]
        public virtual FairRandom FairRandom { get; set; } = null!;

        [InverseProperty(nameof(UpgradeResult.RandomTransaction))]
        public virtual ICollection<UpgradeResult> UpgradeResults { get; set; } = null!;

        [ForeignKey(nameof(CaseId))]
        [InverseProperty(nameof(Case.RandomTransactions))]
        public virtual Case? Case { get; set; }
    }
}