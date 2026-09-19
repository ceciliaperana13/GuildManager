using System.ComponentModel;

namespace GuildManager.Client.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private IScreenViewModel _currentView = null!;
        public IScreenViewModel CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(nameof(CurrentView)); }
        }

        public MainWindowViewModel()
        {
            CurrentView = new MainMenuViewModel(); // écran de démarrage : Jouer / Options / Quitter
        }

        public void NavigateTo(IScreenViewModel viewModel) => CurrentView = viewModel;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}