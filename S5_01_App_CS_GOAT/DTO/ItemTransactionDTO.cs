using AutoMapper;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;

namespace S5_01_App_CS_GOAT.DTO
{
    public class ItemTransactionDTO
    {
       public DateTime TransactionDate { get; set; }
        public double WalletValue { get; set; }

        public DateTime? CancelledOn { get; set; }
    }
        
}
         



