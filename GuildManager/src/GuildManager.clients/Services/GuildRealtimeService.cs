using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace GuildManager.Client.Services;

public static class GuildRealtimeService
{
    private static HubConnection? _connection;

    public static event Action<ResourcesResponse>? ResourcesUpdated;

    public static async Task StartAsync(string apiBaseUrl, int guildId = 1)
    {
        if (_connection is not null)
            return; // déjà démarré

        _connection = new HubConnectionBuilder()
            .WithUrl($"{apiBaseUrl}/hubs/guild")
            .WithAutomaticReconnect()
            .Build();

        _connection.On<ResourcesResponse>("ResourcesUpdated", resources =>
        {
            ResourcesUpdated?.Invoke(resources);
        });

        await _connection.StartAsync();
        await _connection.InvokeAsync("JoinGuild", guildId);
    }

    public static async Task StopAsync()
    {
        if (_connection is null) return;
        await _connection.DisposeAsync();
        _connection = null;
    }
}