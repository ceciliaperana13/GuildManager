using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.ViewModel;

public class GuildViewModel : IScreenViewModel, IGameAwareViewModel
{
    public string BackgroundPath => "Assets/UI/guilde_background.png";
    public Game Game { get; private set; } = null!;

    public void SetGame(Game game) => Game = game;
}