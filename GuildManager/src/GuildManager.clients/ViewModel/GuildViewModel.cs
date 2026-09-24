using System.ComponentModel;
using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.ViewModel;

public class GuildViewModel : IScreenViewModel, IGameAwareViewModel, INotifyPropertyChanged
{
    public string BackgroundPath => "Assets/UI/guilde_background.png";

    private Game _game = null!;
    public Game Game
    {
        get => _game;
        private set { _game = value; OnPropertyChanged(nameof(Game)); }
    }

    public int Gold => Game?.gold ?? 0;
    public int Food => Game?.food ?? 0;
    public int Prestige => Game?.prestige ?? 0;

    public void SetGame(Game game)
    {
        Game = game;
        OnPropertyChanged(nameof(Gold));
        OnPropertyChanged(nameof(Food));
        OnPropertyChanged(nameof(Prestige));
    }

    public void RefreshResources()
    {
        OnPropertyChanged(nameof(Gold));
        OnPropertyChanged(nameof(Food));
        OnPropertyChanged(nameof(Prestige));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}