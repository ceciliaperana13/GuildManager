using System;
using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.ViewModel;
using GuildManager.Client.Services;
using GuildManager.Infrastructure.Configurations;
using GuildManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.View;

public partial class PlayMenuView : UserControl
{
    public PlayMenuView()
    {
        InitializeComponent();

    }

    private async void OnSoloClicked(object sender, RoutedEventArgs e)
    {
        SoloButton.IsEnabled = false;
        StatusText.Text = "Connexion à la base locale...";

        try
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var services = new ServiceCollection();
            services.AddInfrastructure(configuration);
            using var provider = services.BuildServiceProvider();
            await using var dbContext = provider.GetRequiredService<GuildManagerDbContext>();

            var canConnect = await dbContext.Database.CanConnectAsync();
            if (!canConnect)
            {
                StatusText.Text = "Impossible de joindre la base PostgreSQL locale.";
                return;
            }

            Console.WriteLine("Mode solo");
            Console.WriteLine("Connexion à la base locale : OK");
            StatusText.Text = "Connexion à la base locale : OK";

            Game game = new Game("test", 1, 0, 10000, 10000, 1, 0);

            var saveSoloService = new SaveSoloService();
            var newSave = saveSoloService.CreateNewSave(game);
            NavigationService.CurrentSaveId = newSave.SaveId;
            Console.WriteLine($"Nouvelle save solo créée : {newSave.SaveId}");

            NavigationService.NavigateTo(new DialogueViewModel("intro_01", "/Assets/UI/guilde_background.png"), game);
            //NavigationService.NavigateTo(new GuildViewModel(), game);
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Erreur base locale : {ex.Message}";
        }
        finally
        {
            SoloButton.IsEnabled = true;

        }
    }
}