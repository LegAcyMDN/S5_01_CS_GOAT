using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// Classe de base pour tous les ViewModels implémentant INotifyPropertyChanged
    /// pour le pattern MVVM dans Blazor
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Déclenche l'événement PropertyChanged pour notifier les changements de propriété
        /// </summary>
        /// <param name="propertyName">Nom de la propriété qui a changé</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Définit la valeur d'une propriété et déclenche PropertyChanged si la valeur change
        /// </summary>
        /// <typeparam name="T">Type de la propriété</typeparam>
        /// <param name="field">Référence au champ de stockage</param>
        /// <param name="value">Nouvelle valeur</param>
        /// <param name="propertyName">Nom de la propriété</param>
        /// <returns>True si la valeur a changé, sinon false</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Méthode appelée lors de l'initialisation du ViewModel
        /// </summary>
        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
