using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Timers;

namespace GuildManager.Client.Services;

public static class SessionKeepAlive
{
    private static HttpClient? _client;
    private static System.Timers.Timer? _timer;

    public static void Start(string apiBaseUrl, string playerName)
    {
        Stop(); // évite les doublons si appelé plusieurs fois

        _client = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };

        _ = _client.PostAsJsonAsync("api/session/hello", new { PlayerName = playerName });

        _timer = new System.Timers.Timer(10000);
        _timer.Elapsed += async (_, _) =>
        {
            try
            {
                await _client.PostAsJsonAsync("api/session/heartbeat", new { PlayerName = playerName });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Heartbeat échoué : {ex.Message}");
            }
        };
        _timer.Start();
    }

    public static async Task<List<OnlinePlayer>> GetOnlinePlayersAsync()
    {
        if (_client is null) return new List<OnlinePlayer>();

        try
        {
            var result = await _client.GetFromJsonAsync<List<OnlinePlayer>>("api/session/online");
            return result ?? new List<OnlinePlayer>();
        }
        catch
        {
            return new List<OnlinePlayer>();
        }
    }

    public static void Stop()
    {
        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;
        _client?.Dispose();
        _client = null;
    }
}

public record OnlinePlayer(string Name, string Ip, DateTime LastSeen);