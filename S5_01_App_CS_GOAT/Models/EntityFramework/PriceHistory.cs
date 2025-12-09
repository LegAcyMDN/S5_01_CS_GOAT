using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_pricehistory_prh")]
    [Index(nameof(PriceDate))]
    public class PriceHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("prh_id")]
        public int PriceHistoryId { get; set; }

        [Required]
        [Column("prh_pricedate")]
        public DateTime PriceDate { get; set; }

        [Required]
        [Column("prh_pricevalue")]
        [Range(0.0, double.MaxValue)]
        public double PriceValue { get; set; }

        [Column("prh_guessdate")]
        public DateTime? GuessDate { get; set; }

        [Required]
        [Column("wrt_id")]
        public int WearTypeId { get; set; }

        [Required]
        [Column("skn_id")]
        public int SkinId { get; set; } 

        [ForeignKey(nameof(WearTypeId))]
        [InverseProperty(nameof(WearType.PriceHistories))]
        public virtual WearType WearType { get; set; } = null!;

        [ForeignKey(nameof(SkinId))]
        [InverseProperty(nameof(Skin.PriceHistories))]
        public virtual Skin Skin { get; set; } = null!;
    }
}