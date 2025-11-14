using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    [Table("t_e_moneytransaction_mtr")]
    public class MoneyTransaction : Transaction
    {
        [Required]
        [Column("pmt_id")]
        public int PaymentMethodId { get; set; }

        [ForeignKey(nameof(PaymentMethodId))]
        [InverseProperty(nameof(PaymentMethod.MoneyTransactions))]
        public virtual PaymentMethod PaymentMethod { get; set; } = null!;
    }
}