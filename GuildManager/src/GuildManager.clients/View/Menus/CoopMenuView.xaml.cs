using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using GuildManager.Api.Hubs;
using GuildManager.Api.Models;
using GuildManager.Api.Services;
using GuildManager.Aplication.Guilds.Controls;
using GuildManager.Client.Services;
using GuildManager.Client.ViewModel;
using GuildManager.Infrastructure.Configurations;
using GuildManager.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GuildManager.Client.View;

public partial class CoopMenuView : UserControl
{
    private static WebApplication? _apiApp; // static : reste vivante tant que l'appli tourne

    public CoopMenuView()
    {
        InitializeComponent();
        Loaded += CoopMenuView_Loaded;
    }

    private async void CoopMenuView_Loaded(object sender, RoutedEventArgs e)
    {
        if (_apiApp is not null)
        {
            ShowHostReady();
            return;
        }

        LocalIpText.Text = "Démarrage de votre partie...";

        try
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                ContentRootPath = AppContext.BaseDirectory
            });

            builder.Services.AddControllers();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddSingleton<PlayerConnectionTracker>();
            builder.Services.AddHostedService<PlayerStatusReporter>();
            builder.Services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();

            // Sauvegarde / coop / temps réel 
            builder.Services.AddSignalR();

            builder.Services.AddKeyedSingleton<ISaveFileStore>("solo",
                (_, _) => new JsonSaveFileStore(System.IO.Path.Combine("data", "savesolo.json")));

            builder.Services.AddKeyedSingleton<ISaveFileStore>("coop",
                (_, _) => new JsonSaveFileStore(System.IO.Path.Combine("data", "saves.json")));

            // Instance serveur partagée : la liste des candidats au recrutement
            // (adventurersToHire) doit être identique pour tous les joueurs coop.
            builder.Services.AddSingleton<AdventurerManager>();

            builder.Services.AddScoped<IGuildMembershipService, GuildMembershipService>();
            // 

            var app = builder.Build();
            app.MapControllers();
            app.MapHub<GuildHub>("/hubs/guild");
            app.Urls.Add("http://0.0.0.0:5080");

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<GuildManagerDbContext>();
                await db.Database.MigrateAsync();
            }

            await app.StartAsync();
            _apiApp = app;

            AppSession.ApiBaseUrl = "http://localhost:5080";
            AppSession.IsHost = true;
            AppSession.IsCoop = true;

            ShowHostReady();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Impossible de démarrer votre partie (port 5080 déjà utilisé, base de données injoignable, etc.).\n\n{ex.Message}",
                "GuildManager - Erreur de démarrage",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            LocalIpText.Text = "Échec du démarrage de votre partie.";
        }
    }

    private void ShowHostReady()
    {
        var localIp = Dns.GetHostEntry(Dns.GetHostName()).AddressList
            .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

        LocalIpText.Text = localIp is not null
            ? $"Votre partie est hébergée. Donnez cette adresse aux autres joueurs :\n{localIp}"
            : "Votre partie est hébergée, mais votre IP locale n'a pas pu être détectée.";

        EnterGameButton.Visibility = Visibility.Visible;
    }

    private void EnterGameButton_Click(object sender, RoutedEventArgs e)
        => NavigationService.NavigateTo(new AuthChoiceViewModel());

    private void ShowJoinPanel_Click(object sender, RoutedEventArgs e)
    {
        JoinPanel.Visibility = Visibility.Visible;
        HostIpTextBox.Focus();
    }

    private async void JoinButton_Click(object sender, RoutedEventArgs e)
    {
        var hostIp = HostIpTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(hostIp))
        {
            StatusText.Text = "Saisissez l'adresse IP de l'hôte.";
            return;
        }

        var remoteUrl = $"http://{hostIp}:5080";
        StatusText.Text = "Vérification de l'hôte distant...";

        try
        {
            using var client = new HttpClient { BaseAddress = new Uri(remoteUrl), Timeout = TimeSpan.FromSeconds(5) };
            var response = await client.GetAsync("api/health");

            if (!response.IsSuccessStatusCode)
            {
                StatusText.Text = "Impossible de joindre cet hôte.";
                return;
            }
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Impossible de joindre l'hôte : {ex.Message}";
            return;
        }

        AppSession.ApiBaseUrl = remoteUrl;
        AppSession.IsHost = false;
        AppSession.IsCoop = true;
        AppSession.UserId = 0;
        AppSession.Username = string.Empty;

        StatusText.Text = "Connecté à l'hôte.";
        NavigationService.NavigateTo(new AuthChoiceViewModel());
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
        => NavigationService.GoBack();
}