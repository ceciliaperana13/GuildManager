using GuildManager.Api.Models;

namespace GuildManager.Api.Services;

public interface IGuildMembershipService
{
    
    Task<GuildSaveState> AddMemberIfNotExistsAsync(string username);
}