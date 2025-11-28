using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_itemtransaction_itr")]
    public class ItemTransaction : Transaction
    {
        [Required]
        [Column("inv_id")]
        public int InventoryItemId { get; set; }

        [ForeignKey(nameof(InventoryItemId))]
        [InverseProperty(nameof(InventoryItem.ItemTransactions))]
        public virtual InventoryItem InventoryItem { get; set; } = null!;
    }
}