using GuildManager.Api.Models;

namespace GuildManager.Api.Services;

public interface ISaveFileStore
{
    Task<GuildSaveState> LoadAsync();
    Task SaveAsync(GuildSaveState state);

    
    // Reads, modifies, and saves the state within a single critical section,
    // to prevent lost updates in co-op (two players buying at the same time).
    Task<GuildSaveState> UpdateAsync(Action<GuildSaveState> mutate);
}