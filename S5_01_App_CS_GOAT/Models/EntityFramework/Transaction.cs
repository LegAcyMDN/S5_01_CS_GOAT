using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_transaction_txn")]
    [Index(nameof(TransactionDate))]
    [Index(nameof(CancelledOn))]
    public abstract partial class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("txn_id")]
        public int TransactionId { get; set; }

        [Required]
        [Column("txn_transactiondate")]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Required]
        [Column("txn_walletvalue")]
        [Range(0.0, double.MaxValue)]
        public double WalletValue { get; set; }

        [Column("txn_cancelledon")]
        public DateTime? CancelledOn { get; set; }

        [Required]
        [Column("usr_id")]
        public int UserId { get; set; }

        [Column("ntf_id")]
        public int? NotificationId { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.Transactions))]
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(NotificationId))]
        [InverseProperty(nameof(Notification.Transactions))]
        public virtual Notification? Notification { get; set; }
    }
}