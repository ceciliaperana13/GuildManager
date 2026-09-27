using System.ComponentModel;
using System.Windows;
using GuildManager.Aplication.Guilds.Controls;
using GuildManager.Client.Services;

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
        // Si on avait déjà une souscription (revenue sur cet écran après
        // une navigation), on se désinscrit d'abord pour éviter les doublons.
        GuildRealtimeService.ResourcesUpdated -= OnResourcesUpdated;

        Game = game;
        OnPropertyChanged(nameof(Gold));
        OnPropertyChanged(nameof(Food));
        OnPropertyChanged(nameof(Prestige));

        // On se réabonne pour être notifié des futurs changements de ressources.
        GuildRealtimeService.ResourcesUpdated += OnResourcesUpdated;
    }

    private void OnResourcesUpdated(ResourcesResponse resources)
    {
        Game.SyncResources(resources.Gold, resources.Food);
        RefreshResources();
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