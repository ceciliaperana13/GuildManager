using GuildManager.Api.Models;

namespace GuildManager.Api.Services;

public interface ISaveFileStore
{
    Task<GuildSaveState> LoadAsync();
    Task SaveAsync(GuildSaveState state);
}