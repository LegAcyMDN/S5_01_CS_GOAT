using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_promocode_prc")]
    [Index(nameof(Code))]
    [Index(nameof(ExpiryDate))]
    public partial class PromoCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("prc_id")]
        public int PromoCodeId { get; set; }

        [Required]
        [Column("prc_code")]
        [StringLength(50)]
        public string Code { get; set; } = SecurityService.GenerateSeed(8);

        [Column("prc_remaininguses")]
        public int? RemainingUses { get; set; }

        [Required]
        [Column("prc_discountpercentage")]
        [Range(1, 100)]
        public int? DiscountPercentage { get; set; }

        [Required]
        [Column("prc_discountamount")]
        [Range(0.0, double.MaxValue)]
        public double? DiscountAmount { get; set; }  

        [Column("prc_validitystart")]
        public DateTime? ValidityStart { get; set; } = DateTime.Now;

        [Column("prc_expirydate")]
        public DateTime? ExpiryDate { get; set; }

        [Column("prc_refreshdelay")]
        public TimeSpan? RefreshDelay { get; set; }

        [Column("cas_id")]
        public int? CaseId { get; set; }

        [Column("usr_id")]
        public int? UserId { get; set; }

        [ForeignKey(nameof(CaseId))]
        [InverseProperty(nameof(Case.PromoCodes))]
        public virtual Case? Case { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.PromoCodes))]
        public virtual User? User { get; set; }
    }
}