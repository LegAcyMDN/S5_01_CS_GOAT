using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_rarity_rar")]
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

        [InverseProperty(nameof(Skin.Rarity))]
        public virtual ICollection<Skin> Skins { get; set; } = null!;
    }
}