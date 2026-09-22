using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.ViewModel;

public class ReceptionViewModel : IScreenViewModel, IGameAwareViewModel
{
    public string BackgroundPath => "Assets/UI/Table_avanturier.png";
    public Game Game { get; private set; } = null!;

    public void SetGame(Game game) => Game = game;
}