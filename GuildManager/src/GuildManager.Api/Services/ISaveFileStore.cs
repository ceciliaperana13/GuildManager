using GuildManager.Api.Models;

namespace GuildManager.Api.Services;

public interface ISaveFileStore
{
    Task<GuildSaveState> LoadAsync();
    Task SaveAsync(GuildSaveState state);

    // Lit, modifie et sauve l'état en une seule section critique,
    // pour éviter les lost updates en coop (deux joueurs qui achètent en même temps).
    Task<GuildSaveState> UpdateAsync(Action<GuildSaveState> mutate);
}