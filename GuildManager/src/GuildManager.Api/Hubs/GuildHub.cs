using Microsoft.AspNetCore.SignalR;

namespace GuildManager.Api.Hubs;



/// SignalR hub for the co-op guild. Each client joins a group corresponding to their guild,
/// and receives events pushed by the server (shared resources, members, etc.).
public class GuildHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"[GuildHub] Connexion établie : {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"[GuildHub] Déconnexion : {Context.ConnectionId} ({exception?.Message})");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinGuild(int guildId)
    {
        Console.WriteLine($"[GuildHub] {Context.ConnectionId} rejoint le groupe guild-{guildId}");
        await Groups.AddToGroupAsync(Context.ConnectionId, GuildGroup(guildId));
    }

    public async Task LeaveGuild(int guildId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GuildGroup(guildId));
    }

    public static string GuildGroup(int guildId) => $"guild-{guildId}";
}