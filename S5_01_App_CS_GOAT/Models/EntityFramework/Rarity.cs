using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_rarity_rar")]
    [Index(nameof(RarityName), IsUnique = true)]
    public class Rarity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("rar_id")]
        public int RarityId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("rar_rarityname")]
        public string RarityName { get; set; } = null!;

        [Required]
        [Column("rar_raritycolor")]
        [RegularExpression("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")]
        public string RarityColor { get; set; } = null!;

        [InverseProperty(nameof(Skin.Rarity))]
        public virtual ICollection<Skin> Skins { get; set; } = null!;
    }
}