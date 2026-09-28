using System.Threading.Tasks;
using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.Services;

public static class CoopSync
{
    // Envoie au pot commun tout ce qui a changé localement (gain ou perte).
    // Retourne false si l'envoi a échoué.
    public static async Task<bool> PushAsync(Game game)
    {
        if (!game.IsCoop) return true;

        var (goldDelta, foodDelta) = game.PendingCoopDelta();
        if (goldDelta == 0 && foodDelta == 0) return true;

        var (success, resources, _) = await new GuildApiClient()
            .AdjustResourcesAsync(goldDelta, foodDelta);

        if (success)
            game.CommitResources(resources!.Gold, resources.Food);

        return success;
    }
}