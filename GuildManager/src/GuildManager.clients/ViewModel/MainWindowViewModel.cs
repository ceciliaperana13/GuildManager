using System.ComponentModel;
using GuildManager.Aplication.Guilds.Controls;

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

        public Game? CurrentGame { get; private set; }

        public MainWindowViewModel()
        {
            CurrentView = new MainMenuViewModel();
        }

        public void NavigateTo(IScreenViewModel viewModel, Game game)
        {
            CurrentGame = game;
            if (viewModel is IGameAwareViewModel gameAware)
                gameAware.SetGame(game);

            CurrentView = viewModel;
        }

        public void NavigateTo(IScreenViewModel viewModel)
        {
            if (viewModel is IGameAwareViewModel gameAware && CurrentGame != null)
                gameAware.SetGame(CurrentGame);

            CurrentView = viewModel;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}