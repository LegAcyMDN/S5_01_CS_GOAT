using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_fairrandom_frn")]
    public class FairRandom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("frn_id")]
        public int FairRandomId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("frn_serverseed")]
        public string ServerSeed { get; set; } = null!;

        [Required]
        [StringLength(255)]
        [Column("frn_serverhash")]
        public string ServerHash { get; set; } = null!;

        [Required]
        [Column("frn_usernonce")]
        public int UserNonce { get; set; }

        [Required]
        [StringLength(255)]
        [Column("frn_combinedhash")]
        public string CombinedHash { get; set; } = null!;

        [Required]
        [Column("frn_fraction")]
        [Range(0.0, 1.0)]
        public double Fraction { get; set; }

        [Column("rtr_id")]
        public int? RandomTransactionId { get; set; }

        [Column("usr_id_upgrade")]
        public int? UserIdUpgrade { get; set; }

        [Column("wer_id_upgrade")]
        public int? WearIdUpgrade { get; set; }

        [ForeignKey(nameof(RandomTransactionId))]
        [InverseProperty(nameof(RandomTransaction.FairRandoms))]
        public virtual RandomTransaction? RandomTransaction { get; set; }

        [ForeignKey($"{nameof(UserIdUpgrade)},{nameof(WearIdUpgrade)}")]
        [InverseProperty(nameof(UpgradeResult.FairRandom))]
        public virtual UpgradeResult? UpgradeResult { get; set; }
    }
}