using S5_01_Blazor_CS_GOAT.Models;
using Microsoft.JSInterop;
using Shared.DTO;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour CaseRollComponent - Gère l'animation de roulette d'ouverture de caisse
    /// </summary>
    public class CaseRollComponentViewModel : ViewModelBase
    {
        private readonly IJSRuntime _jsRuntime;
        private static readonly Random _random = new();

        private List<SkinDTO> _listSkins = new();
        private List<SkinDTO> _listSkinsInCaseRoll = new();

        public CaseRollComponentViewModel(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public List<SkinDTO> ListSkins
        {
            get => _listSkins;
            set => SetProperty(ref _listSkins, value);
        }

        public List<SkinDTO> ListSkinsInCaseRoll
        {
            get => _listSkinsInCaseRoll;
            set => SetProperty(ref _listSkinsInCaseRoll, value);
        }

        public override async Task InitializeAsync()
        {
            await OpenCaseAsync();
        }

        /// <summary>
        /// Génère la séquence d'items pour l'animation de roulette
        /// </summary>
        public async Task OpenCaseAsync()
        {
            if (ListSkins.Count == 0)
                return;

            ListSkinsInCaseRoll.Clear();

            ListSkinsInCaseRoll = ListSkins;

            // // Ajouter 72 items aléatoires avant le résultat
            // for (int i = 0; i <= 71; i++)
            // {
            //     int indexOfListToAdd = _random.Next(ListSkins.Count);
            //     SkinDTO itemWithWeight = ListSkins[indexOfListToAdd];
            //     ListSkinsInCaseRoll.Add(itemWithWeight);
            // }
            //
            // // Ajouter le skin gagné
            // ListSkinsInCaseRoll.Add(ListSkins[0]);
            // await _jsRuntime.InvokeVoidAsync("eval", $"console.log('item to win : ' + '{ListSkins[0].ItemName} | {ListSkins[0].SkinName}')");
            //
            // foreach (var oneSkin in ListSkins)
            // {
            //     await _jsRuntime.InvokeVoidAsync("eval", $"console.log('{oneSkin.ItemName} | {oneSkin.SkinName}')");
            // }
            //
            // // Ajouter 10 items aléatoires après le résultat
            // for (int i = 0; i <= 9; i++)
            // {
            //     int indexOfListToAdd = _random.Next(ListSkins.Count);
            //     Skin itemWithWeight = ListSkins[indexOfListToAdd];
            //     ListSkinsInCaseRoll.Add(itemWithWeight);
            //
            //     await _jsRuntime.InvokeVoidAsync("caseRollLogic.printStuff");
            // }

            // Déclencher l'animation de roulette
            await _jsRuntime.InvokeVoidAsync("caseRollLogic.rollForItem");
        }

        /// <summary>
        /// Déclenche l'animation de roulette
        /// </summary>
        public async Task RollAsync()
        {
            await _jsRuntime.InvokeVoidAsync("CaseRollComponent.rollForItem", "myElement");
        }
    }
}
