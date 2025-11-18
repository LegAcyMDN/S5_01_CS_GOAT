using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_paymentmethod_pmt")]
    [Index(nameof(PaymentMethodName), IsUnique = true)]
    public class PaymentMethod
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("pmt_id")]
        public int PaymentMethodId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("pmt_paymentmethodname")]
        public string PaymentMethodName { get; set; } = null!;

        [Required]
        [Column("pmt_fromwallet")]
        public bool FromWallet { get; set; }

        [Required]
        [Column("pmt_towallet")]
        public bool ToWallet { get; set; }

        [InverseProperty(nameof(MoneyTransaction.PaymentMethod))]
        public virtual ICollection<MoneyTransaction> MoneyTransactions { get; set; } = null!;
    }
}