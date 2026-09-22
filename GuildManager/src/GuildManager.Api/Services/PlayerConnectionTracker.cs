using System.Collections.Concurrent;

namespace GuildManager.Api.Services;

public class PlayerConnectionTracker
{
    private readonly ConcurrentDictionary<string, (string Ip, DateTime LastSeen)> _players = new();

    public void Register(string playerName, string ip)
        => _players[playerName] = (ip, DateTime.UtcNow);

    public List<object> GetOnlinePlayers()
    {
        var cutoff = DateTime.UtcNow.AddSeconds(-30); // considéré déconnecté après 30s sans heartbeat
        return _players
            .Where(p => p.Value.LastSeen >= cutoff)
            .Select(p => new { Name = p.Key, p.Value.Ip, p.Value.LastSeen })
            .Cast<object>()
            .ToList();
    }
}