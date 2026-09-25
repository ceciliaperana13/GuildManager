using GuildManager.Api.Hubs;
using GuildManager.Api.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GuildManager.Api.Services;

public class GuildMembershipService : IGuildMembershipService
{
    private readonly ISaveFileStore _coopStore;
    private readonly IHubContext<GuildHub> _hub;
    private readonly ILogger<GuildMembershipService> _logger;

    public GuildMembershipService(
        [FromKeyedServices("coop")] ISaveFileStore coopStore,
        IHubContext<GuildHub> hub,
        ILogger<GuildMembershipService> logger)
    {
        _coopStore = coopStore;
        _hub = hub;
        _logger = logger;
    }

    public async Task<GuildSaveState> AddMemberIfNotExistsAsync(string username)
    {
        var state = await _coopStore.LoadAsync();

        var alreadyMember = state.Members.Any(m =>
            m.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        if (!alreadyMember)
        {
            state.Members.Add(new GuildMemberDto(username, DateTime.UtcNow));
            await _coopStore.SaveAsync(state);

            _logger.LogInformation("{Username} a rejoint la guilde coop", username);

            await _hub.Clients.Group(GuildHub.GuildGroup(state.GuildId))
                .SendAsync("MemberJoined", new { username, joinedAt = DateTime.UtcNow });
        }

        return state;
    }
}