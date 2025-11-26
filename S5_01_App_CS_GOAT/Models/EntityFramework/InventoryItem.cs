using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_j_inventoryitem_inv")]
    [Index(nameof(AcquiredOn))]
    [Index(nameof(RemovedOn))]
    [Index(nameof(IsFavorite))]
    public partial class InventoryItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("inv_id")]
        public int InventoryItemId { get; set; }

        [Required]
        [Column("usr_id")]
        public int UserId { get; set; }

        [Required]
        [Column("wer_id")]
        public int WearId { get; set; }

        [Required]
        [Column("inv_float")]
        [Range(0.0, 1.0)]
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