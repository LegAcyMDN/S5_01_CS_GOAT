using S5_01_Blazor_CS_GOAT.Models;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour WeaponDisplayComponent - Gère l'affichage d'un skin d'arme
    /// </summary>
    public class WeaponDisplayComponentViewModel : ViewModelBase
    {
        private string _imageLink = string.Empty;
        private Skin? _weaponSkin;
        private string _height = "100px";
        private string _width = "100px";

        public string ImageLink
        {
            get => _imageLink;
            set => SetProperty(ref _imageLink, value);
        }

        public Skin? WeaponSkin
        {
            get => _weaponSkin;
            set => SetProperty(ref _weaponSkin, value);
        }

        public string Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        public string Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        /// <summary>
        /// Obtient la couleur hexadécimale de la rareté
        /// </summary>
        public string GetRarityHexColorFromInt(int rarityInt)
        {
            return rarityInt switch
            {
                1 => "#afafaf",
                2 => "#6496e1",
                3 => "#4b69cd",
                4 => "#8847ff",
                5 => "#d32ce6",
                6 => "#eb4b4b",
                7 => "#afafaf",
                _ => "#afafaf"
            };
        }
    }
}
