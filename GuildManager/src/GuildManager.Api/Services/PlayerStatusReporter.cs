using Microsoft.Extensions.Hosting;

namespace GuildManager.Api.Services;

public class PlayerStatusReporter : BackgroundService
{
    private readonly PlayerConnectionTracker _tracker;

    public PlayerStatusReporter(PlayerConnectionTracker tracker) => _tracker = tracker;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);

            var players = _tracker.GetOnlinePlayers();

            Console.WriteLine();
            Console.WriteLine($"--- [{DateTime.Now:HH:mm:ss}] Joueurs connectés : {players.Count} ---");
            foreach (dynamic p in players)
                Console.WriteLine($"   - {p.Name} ({p.Ip})");
            if (players.Count == 0)
                Console.WriteLine("   (aucun)");
            Console.WriteLine();
        }
    }
}