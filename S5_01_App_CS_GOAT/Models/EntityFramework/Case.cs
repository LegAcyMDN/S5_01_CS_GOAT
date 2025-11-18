using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_case_cas")]
    public class Case
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("cas_id")]
        public int CaseId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("cas_casename")]
        public string CaseName { get; set; } = null!;

        [Required]
        [StringLength(255)]
        [Column("cas_caseimage")]
        public string CaseImage { get; set; } = null!;

        [Required]
        [Column("cas_caseprice")]
        public double CasePrice { get; set; }

        [InverseProperty(nameof(CaseContent.Case))]
        public virtual ICollection<CaseContent> CaseContents { get; set; } = null!;

        [InverseProperty(nameof(Favorite.Case))]
        public virtual ICollection<Favorite> Favorites { get; set; } = null!;

        [InverseProperty(nameof(RandomTransaction.Case))]
        public virtual ICollection<RandomTransaction> RandomTransactions { get; set; } = null!;
    }
}