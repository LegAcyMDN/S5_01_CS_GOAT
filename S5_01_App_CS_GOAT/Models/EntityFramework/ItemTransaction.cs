using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_itemtransaction_itr")]
    public class ItemTransaction : Transaction
    {
        [Column("usr_id_item")]
        public int? UserIdItem { get; set; }

        [Column("wer_id")]
        public int? WearIdItem { get; set; }

        [ForeignKey($"{nameof(UserIdItem)},{nameof(WearIdItem)}")]
        [InverseProperty(nameof(InventoryItem.ItemTransactions))]
        public virtual InventoryItem? InventoryItem { get; set; }
    }
}