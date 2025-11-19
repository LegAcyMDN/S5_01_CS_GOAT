using S5_01_App_CS_GOAT.Models.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace S5_01_App_CS_GOAT.DTO
{
    public class FairRandomDTO
    {
     
        public string ServerSeed { get; set; } = null!;

   
        public string ServerHash { get; set; } = null!;

        public int UserNonce { get; set; }

      
        public string CombinedHash { get; set; } = null!;


        public double Fraction { get; set; }

        public int? RandomTransactionId { get; set; }


        public int? UserIdUpgrade { get; set; }

        public int? WearIdUpgrade { get; set; }

      
    }
}
