using S5_01_Blazor_CS_GOAT.Models;
using Microsoft.AspNetCore.Components;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour CaseComponent - Gère l'affichage et la navigation d'une caisse
    /// </summary>
    public class CaseComponentViewModel : ViewModelBase
    {
        private readonly NavigationManager _navigation;
        private Case? _caseObject;

        public CaseComponentViewModel(NavigationManager navigation)
        {
            _navigation = navigation;
        }

        public Case? CaseObject
        {
            get => _caseObject;
            set => SetProperty(ref _caseObject, value);
        }

        /// <summary>
        /// Navigue vers la page de détail de la caisse
        /// </summary>
        public void NavigateToCase()
        {
            if (CaseObject != null)
            {
                _navigation.NavigateTo($"/caseview/{CaseObject.CaseId}");
            }
        }
    }
}
