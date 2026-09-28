using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.ViewModel;

public class DefeatViewModel : IScreenViewModel
{
    public string BackgroundPath => "/Assets/UI/Defeat_screen.png";
    public Game Game { get; }
    public string Reason { get; }

    public DefeatViewModel(Game game, string reason = "") 
    {
        Game = game;
        Reason = reason;
    }
}