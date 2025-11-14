using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_randomtransaction_rtr")]
    public class RandomTransaction : Transaction
    {
        [Required]
        [StringLength(255)]
        [Column("rtr_userseed")]
        public string UserSeed { get; set; } = null!;

        [Column("usr_id_upgrade")]
        public int? UserIdUpgrade { get; set; }

        [Column("wer_id_upgrade")]
        public int? WearIdUpgrade { get; set; }

        [Column("cas_id")]
        public int? CaseId { get; set; }

        [InverseProperty(nameof(FairRandom.RandomTransaction))]
        public virtual ICollection<FairRandom> FairRandoms { get; set; }

        [ForeignKey($"{nameof(UserIdUpgrade)},{nameof(WearIdUpgrade)}")]
        [InverseProperty(nameof(UpgradeResult.RandomTransactions))]
        public virtual UpgradeResult? UpgradeResult { get; set; }

        [ForeignKey(nameof(CaseId))]
        [InverseProperty(nameof(Case.RandomTransactions))]
        public virtual Case? Case { get; set; }
    }
}