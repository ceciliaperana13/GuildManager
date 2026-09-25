using Microsoft.AspNetCore.SignalR;

namespace GuildManager.Api.Hubs;


/// Hub SignalR pour la guilde coop. Chaque client rejoint un groupe = sa guilde,
/// et reçoit les events poussés par le serveur (ressources partagées, membres, etc.).
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