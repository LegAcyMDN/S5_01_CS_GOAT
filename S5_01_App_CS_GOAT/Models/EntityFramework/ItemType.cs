using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_itemtype_itt")]
    [Index(nameof(ItemTypeName), IsUnique = true)]
    public partial class ItemType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("itt_id")]
        public int ItemTypeId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("itt_itemtypename")]
        public string ItemTypeName { get; set; } = null!;

        [Column("itt_parentid")]
        public int? ParentItemTypeId { get; set; }

        [ForeignKey(nameof(ParentItemTypeId))]
        [InverseProperty(nameof(SubItemTypes))]
        public virtual ItemType? ParentItemType { get; set; }

        [InverseProperty(nameof(ParentItemType))]
        public virtual ICollection<ItemType> SubItemTypes { get; set; } = null!;

        [InverseProperty(nameof(Item.ItemType))]
        public virtual ICollection<Item> Items { get; set; } = null!;
    }
}