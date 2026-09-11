using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using GuildManager.Api.Services;
using GuildManager.Infrastructure.Configurations;
using GuildManager.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MonProjet.View;

namespace MonProjet;

public partial class MainWindow : Window
{
    private WebApplication? _apiApp;
    private System.Timers.Timer? _heartbeatTimer;

    public MainWindow()
    {
        InitializeComponent();
        StartupPanel.ModeSelected += StartupPanel_ModeSelected;
    }

    private async void StartupPanel_ModeSelected(StartupMode mode, string hostIp)
    {
        if (mode == StartupMode.Solo)
        {
            StartupPanel.SetStatus("Connexion à la base locale...");

            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();

                var services = new ServiceCollection();
                services.AddInfrastructure(configuration);
                var provider = services.BuildServiceProvider();
                var dbContext = provider.GetRequiredService<GuildManagerDbContext>();

                var canConnect = await dbContext.Database.CanConnectAsync();
                if (!canConnect)
                {
                    StartupPanel.SetStatus("Impossible de joindre la base PostgreSQL locale.");
                    return;
                }

                Console.WriteLine(" Mode solo");
                Console.WriteLine("Connexion à la base locale : OK");
            }
            catch (Exception ex)
            {
                StartupPanel.SetStatus($"Erreur base locale : {ex.Message}");
                return;
            }

            StartupPanel.Visibility = Visibility.Collapsed;
            GameMenuPanel.Visibility = Visibility.Visible;
            return;
        }

        // Multijoueur
        var isHost = mode == StartupMode.Host;
        StartupPanel.SetStatus("Connexion en cours...");

        var apiBaseUrl = isHost ? "http://localhost:5080" : $"http://{hostIp}:5080";

        if (isHost)
        {
            Console.WriteLine("__Mode Hote__");

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

            _ = _apiApp.RunAsync("http://0.0.0.0:5080");
            Console.WriteLine("API démarrée sur http://0.0.0.0:5080");
        }
        else
        {
            Console.WriteLine(" Mode invité");
            Console.WriteLine($"Connexion à l'hôte : {apiBaseUrl}");
        }

        var sessionClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
        var playerName = Environment.MachineName;

        try
        {
            var response = await sessionClient.PostAsJsonAsync("api/session/hello", new { PlayerName = playerName });
            if (!response.IsSuccessStatusCode)
            {
                StartupPanel.SetStatus($"Échec de connexion ({response.StatusCode})");
                return;
            }
            Console.WriteLine($"Connecté en tant que : {playerName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Impossible de joindre l'hôte : {ex.Message}");
            StartupPanel.SetStatus("Impossible de joindre l'hôte. Vérifie l'IP et le pare-feu.");
            return;
        }

        _heartbeatTimer = new System.Timers.Timer(10000);
        _heartbeatTimer.Elapsed += async (_, _) =>
        {
            try { await sessionClient.PostAsJsonAsync("api/session/heartbeat", new { PlayerName = playerName }); }
            catch (Exception ex) { Console.WriteLine($"Heartbeat échoué : {ex.Message}"); }
        };
        _heartbeatTimer.Start();

        StartupPanel.Visibility = Visibility.Collapsed;
        GameMenuPanel.Visibility = Visibility.Visible;
    }

    protected override async void OnClosed(EventArgs e)
    {
        _heartbeatTimer?.Stop();
        if (_apiApp is not null)
            await _apiApp.StopAsync();

        base.OnClosed(e);
    }
}