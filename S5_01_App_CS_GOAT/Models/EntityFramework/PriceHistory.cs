using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_pricehistory_prc")]
    public class PriceHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("prc_id")]
        public int PriceHistoryId { get; set; }

        [Required]
        [Column("prc_pricedate")]
        public DateTime PriceDate { get; set; } = null!;

        [Required]
        [Column("prc_pricevalue")]
        public double PriceValue { get; set; }

        [Column("prc_guessdate")]
        public DateTime? GuessDate { get; set; }

        [Required]
        [Column("wer_id")]
        public int WearId { get; set; }

        [ForeignKey(nameof(WearId))]
        [InverseProperty(nameof(Wear.PriceHistories))]
        public virtual Wear Wear { get; set; } = null!;
    }
}