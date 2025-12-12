using S5_01_App_CS_GOAT.Services;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_fairrandom_frn")]
    public partial class FairRandom
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

        [Column("usr_id")]
        public int? UserId { get; set; }

        [Column("usr_seed")]
        [StringLength(16)]
        [RegularExpression("^[A-Za-z0-9]{16}$")]
        public string? UserSeed { get; set; }

        [Column("frn_usernonce")]
        public int? UserNonce { get; set; }

        [StringLength(255)]
        [Column("frn_combinedhash")]
        public string? CombinedHash { get; set; }

        [Column("frn_fraction")]
        [Range(0.0, 1.0)]
        public double? Fraction { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.FairRandom))]
        public virtual User? User { get; set; }

        [InverseProperty(nameof(RandomTransaction.FairRandom))]
        public virtual RandomTransaction? RandomTransaction { get; set; }

        [InverseProperty(nameof(UpgradeResult.FairRandom))]
        public virtual UpgradeResult? UpgradeResult { get; set; }
    }
}