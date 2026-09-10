using System;
using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using GuildManager.Api.Services;
using GuildManager.Infrastructure.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MonProjet;

public partial class App : Application
{
    private WebApplication? _apiApp;
    private System.Timers.Timer? _heartbeatTimer;

    // Permet d'ouvrir une fenêtre console à côté de la fenêtre WPF,
    // pour y voir les logs (joueurs connectés, etc.)
    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        AllocConsole();
        Console.Title = "GuildManager - Logs";
        Console.WriteLine(" Démarrage de l'API");

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ContentRootPath = AppContext.BaseDirectory
        });

        builder.Services.AddControllers();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddSingleton<PlayerConnectionTracker>(); // <-- nouveau

        _apiApp = builder.Build();
        _apiApp.MapControllers();

        _ = _apiApp.RunAsync("http://0.0.0.0:5080");

        Console.WriteLine("API démarrée sur http://0.0.0.0:5080");

        //  Nouveau : on s'enregistre nous-mêmes comme joueur connecté
     
        var sessionClient = new HttpClient { BaseAddress = new Uri("http://localhost:5080") };
        var playerName = Environment.MachineName;

        try
        {
            await sessionClient.PostAsJsonAsync("api/session/hello", new { PlayerName = playerName });
            Console.WriteLine($"Connecté en tant que : {playerName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la connexion à la session : {ex.Message}");
        }

        _heartbeatTimer = new System.Timers.Timer(10000);
        _heartbeatTimer.Elapsed += async (_, _) =>
        {
            try
            {
                await sessionClient.PostAsJsonAsync("api/session/heartbeat", new { PlayerName = playerName });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Heartbeat échoué : {ex.Message}");
            }
        };
        _heartbeatTimer.Start();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        _heartbeatTimer?.Stop();

        if (_apiApp is not null)
            await _apiApp.StopAsync();

        base.OnExit(e);
    }
}