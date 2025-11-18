using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_j_inventoryitem_inv")]
    public class InventoryItem
    {
        [Key]
        [Column("usr_id", Order = 1)]
        public int UserId { get; set; }

        [Key]
        [Column("wer_id", Order = 2)]
        public int WearId { get; set; }

        [Required]
        [Column("inv_float")]
        public float Float { get; set; }

        [Required]
        [Column("inv_isfavorite")]
        public bool IsFavorite { get; set; }

        [Required]
        [Column("inv_acquiredon")]
        public DateTime AcquiredOn { get; set; } = DateTime.Now;

        [Column("inv_removedon")]
        public DateTime? RemovedOn { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.InventoryItems))]
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(WearId))]
        [InverseProperty(nameof(Wear.InventoryItems))]
        public virtual Wear Wear { get; set; } = null!;

        [InverseProperty(nameof(UpgradeResult.InventoryItem))]
        public virtual ICollection<UpgradeResult> UpgradeResults { get; set; } = null!;

        [InverseProperty(nameof(ItemTransaction.InventoryItem))]
        public virtual ICollection<ItemTransaction> ItemTransactions { get; set; } = null!;
    }
}