using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace S5_01_App_CS_GOAT.DTO
{
    public class SkinDetailDTO
    {
        // Element of Class Skin
        public string SkinName { get; set; } = null!;

        // Element of Class Rarity
        public string RarityName { get; set; } = null!;

        // Element of Class Wear
        public string WearName { get; set; } = null!;
        public float FloatLow { get; set; }
        public float FloatHigh { get; set; }
        public string Uuid { get; set; } = null!;
    }
}
