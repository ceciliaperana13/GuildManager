using System;
using System.Runtime.InteropServices;
using System.Windows;
using GuildManager.Api.Services;
using GuildManager.Client.Services;
using GuildManager.Infrastructure.Configurations;
using GuildManager.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MonProjet;

public partial class App : Application
{
    private WebApplication? _apiApp;

    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        AllocConsole();
        Console.Title = "GuildManager - Logs";
        Console.WriteLine("Démarrage de l'API locale...");

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ContentRootPath = AppContext.BaseDirectory
        });

        builder.Services.AddControllers();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddSingleton<PlayerConnectionTracker>();
        builder.Services.AddHostedService<PlayerStatusReporter>();
        builder.Services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();

        _apiApp = builder.Build();
        _apiApp.MapControllers();
        _apiApp.Urls.Add("http://0.0.0.0:5080"); // déjà accessible en réseau, même avant de choisir Héberger

        try
        {
            
            using (var scope = _apiApp.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<GuildManagerDbContext>();
                await db.Database.MigrateAsync();
            }

            await _apiApp.StartAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($" Échec du démarrage de l'API locale : {ex}");
            MessageBox.Show(
                $"Impossible de démarrer l'API locale (port 5080 déjà utilisé, base de données injoignable, etc.).\n\n{ex.Message}",
                "GuildManager - Erreur de démarrage",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown();
            return;
        }

        AppSession.ApiBaseUrl = "http://localhost:5080";
        AppSession.IsHost = true; 

        Console.WriteLine("API locale démarrée sur http://0.0.0.0:5080");

        var mainWindow = new MainWindow();
        MainWindow = mainWindow;
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (_apiApp is not null)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
_apiApp.StopAsync(cts.Token).GetAwaiter().GetResult();
            _apiApp.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        base.OnExit(e);
    }
}