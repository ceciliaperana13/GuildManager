using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.ViewModel;

public interface IGameAwareViewModel
{
    void SetGame(Game game);
}