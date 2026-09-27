using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.ViewModel;

public class VictoryViewModel : IScreenViewModel
{
    public string BackgroundPath => "/Assets/UI/Victory_screen.jpg";
    public Game Game { get; }

    public VictoryViewModel(Game game) => Game = game;
}