using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_item_itm")]
    [Index(nameof(ItemName))]
    [Index(nameof(DefIndex))]
    public class Item
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("itm_id")]
        public int ItemId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("itm_itemname")]
        public string ItemName { get; set; } = null!;

        [Required]
        [StringLength(255)]
        [Column("itm_itemmodel")]
        public string ItemModel { get; set; } = null!;

        [Column("itm_defindex")]
        [Range(0, int.MaxValue)]
        public int? DefIndex { get; set; }

        [Required]
        [Column("itt_id")]
        public int ItemTypeId { get; set; }

        [ForeignKey(nameof(ItemTypeId))]
        [InverseProperty(nameof(ItemType.Items))]
        public virtual ItemType ItemType { get; set; } = null!;

        [InverseProperty(nameof(Skin.Item))]
        public virtual ICollection<Skin> Skins { get; set; } = null!;
    }
}