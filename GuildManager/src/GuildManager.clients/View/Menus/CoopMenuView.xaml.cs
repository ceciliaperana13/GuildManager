
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GuildManager.Api.Services;
using GuildManager.Client.Services;
using GuildManager.Client.ViewModel;
using GuildManager.Infrastructure.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace GuildManager.Client.View;

public partial class CoopMenuView : UserControl
{
    private WebApplication? _apiApp;
    private System.Timers.Timer? _heartbeatTimer;
    private HttpClient? _sessionClient;
    private string? _playerName;

    public CoopMenuView()
    {
        InitializeComponent();
    }

    private void ShowJoinPanel_Click(object sender, RoutedEventArgs e)
    {
        JoinPanel.Visibility = Visibility.Visible;
        HostIpTextBox.Focus();
    }

    private async void HostButton_Click(object sender, RoutedEventArgs e)
        => await ConnectAsync(true, "localhost");

    private async void JoinButton_Click(object sender, RoutedEventArgs e)
    {
        var hostIp = HostIpTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(hostIp))
        {
            StatusText.Text = "Saisissez l'adresse IP de l'hôte.";
            return;
        }

        await ConnectAsync(false, hostIp);
    }

    private async Task ConnectAsync(bool isHost, string hostIp)
    {
        SetBusy(true, isHost ? "Démarrage de la partie..." : "Connexion en cours...");
        var apiBaseUrl = $"http://{hostIp}:5080";

        try
        {
            if (isHost)
            {
                var builder = WebApplication.CreateBuilder(new WebApplicationOptions
                {
                    ContentRootPath = AppContext.BaseDirectory
                });
                builder.Services.AddControllers();
                builder.Services.AddInfrastructure(builder.Configuration);
                builder.Services.AddSingleton<PlayerConnectionTracker>();
                builder.Services.AddHostedService<PlayerStatusReporter>();

                _apiApp = builder.Build();
                _apiApp.MapControllers();
                _apiApp.Urls.Add("http://0.0.0.0:5080");
                await _apiApp.StartAsync();
            }

            _sessionClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
            _playerName = Environment.MachineName;
            var response = await _sessionClient.PostAsJsonAsync(
                "api/session/hello", new { PlayerName = _playerName });

            if (!response.IsSuccessStatusCode)
            {
                StatusText.Text = $"Échec de connexion ({response.StatusCode}).";
                await StopApiAsync();
                return;
            }

            _heartbeatTimer = new System.Timers.Timer(10000);
            _heartbeatTimer.Elapsed += async (_, _) =>
            {
                try
                {
                    if (_sessionClient is not null && _playerName is not null)
                        await _sessionClient.PostAsJsonAsync(
                            "api/session/heartbeat", new { PlayerName = _playerName });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Heartbeat échoué : {ex.Message}");
                }
            };
            _heartbeatTimer.Start();
            StatusText.Text = $"Connecté en tant que {_playerName}.";
        }
        catch (Exception ex)
        {
            StatusText.Text = isHost
                ? $"Impossible de démarrer la partie : {ex.Message}"
                : "Impossible de joindre l'hôte. Vérifiez l'IP et le pare-feu.";
            await StopApiAsync();
        }
        finally
        {
            SetBusy(false, StatusText.Text);
        }
    }

    private void SetBusy(bool isBusy, string status)
    {
        HostButton.IsEnabled = !isBusy;
        JoinPanel.IsEnabled = !isBusy;
        StatusText.Text = status;
    }

    private async void BackButton_Click(object sender, RoutedEventArgs e)
    {
        await StopApiAsync();
        NavigationService.NavigateTo(new PlayMenuViewModel());
    }

    private async Task StopApiAsync()
    {
        _heartbeatTimer?.Stop();
        _heartbeatTimer?.Dispose();
        _heartbeatTimer = null;
        _sessionClient?.Dispose();
        _sessionClient = null;

        if (_apiApp is not null)
        {
            await _apiApp.StopAsync();
            await _apiApp.DisposeAsync();
            _apiApp = null;
        }
    }

    private async void UserControl_Unloaded(object sender, RoutedEventArgs e)
        => await StopApiAsync();
}