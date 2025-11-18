using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_j_casecontent_cct")]
    [Index(nameof(Weight))]
    public class CaseContent
    {
        [Key]
        [Column("cas_id", Order = 1)]
        public int CaseId { get; set; }

        [Key]
        [Column("skn_id", Order = 2)]
        public int SkinId { get; set; }

        [Required]
        [Column("cct_weight")]
        [Range(1, int.MaxValue)]
        public int Weight { get; set; }

        [ForeignKey(nameof(CaseId))]
        [InverseProperty(nameof(Case.CaseContents))]
        public virtual Case Case { get; set; } = null!;

        [ForeignKey(nameof(SkinId))]
        [InverseProperty(nameof(Skin.CaseContents))]
        public virtual Skin Skin { get; set; } = null!;
    }
}