using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace GuildManager.Client.Services;

public static class GuildRealtimeService
{
    private static HubConnection? _connection;

    public static event Action<ResourcesResponse>? ResourcesUpdated;
    public static event Action<AdventurerCandidateDto>? AdventurerHired;

    public static async Task StartAsync(string apiBaseUrl, int guildId = 1)
    {
        if (_connection is not null)
        {
            System.Windows.MessageBox.Show($"StartAsync ignoré : connexion déjà existante, état = {_connection.State}");
            return; // déjà démarré
        }

        _connection = new HubConnectionBuilder()
            .WithUrl($"{apiBaseUrl}/hubs/guild")
            .WithAutomaticReconnect()
            .Build();

        _connection.On<ResourcesResponse>("ResourcesUpdated", resources =>
        {
            ResourcesUpdated?.Invoke(resources);
        });

        _connection.On<AdventurerCandidateDto>("AdventurerHired", adventurer =>
        {
            AdventurerHired?.Invoke(adventurer);
        });

        try
        {
            await _connection.StartAsync();
            System.Windows.MessageBox.Show($"SignalR connecté, état = {_connection.State}");

            await _connection.InvokeAsync("JoinGuild", guildId);
            System.Windows.MessageBox.Show($"JoinGuild({guildId}) envoyé");
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Échec connexion SignalR : {ex.Message}");
        }
    }

    public static async Task StopAsync()
    {
        if (_connection is null) return;
        await _connection.DisposeAsync();
        _connection = null;
    }
}