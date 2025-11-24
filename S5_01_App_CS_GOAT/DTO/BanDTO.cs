using S5_01_App_CS_GOAT.Models.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace S5_01_App_CS_GOAT.DTO
{
    public class BanDTO
    {
        public string BanReason { get; set; } = null!;

        public DateTime BanDate { get; set; }

        public int BanDuration { get; set; }

        public string BanTypeName { get; set; } = null!;

    }
}
