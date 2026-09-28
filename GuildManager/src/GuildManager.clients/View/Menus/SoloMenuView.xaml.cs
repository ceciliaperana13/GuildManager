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

public partial class SoloMenuView : UserControl
{
    public SoloMenuView()
    {
        InitializeComponent();
    }

    private async void OnNewGameClicked(object sender, RoutedEventArgs e)
    {
        NewGameButton.IsEnabled = false;
        ContinueButton.IsEnabled = false;
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

            Game game = new Game("test", 1, 0, 500, 500, 1, 0);

            var saveSoloService = new SaveSoloService();
            var newSave = saveSoloService.CreateNewSave(game);
            NavigationService.CurrentSaveId = newSave.SaveId;
            Console.WriteLine($"Nouvelle save solo créée : {newSave.SaveId}");

            NavigationService.NavigateTo(new DialogueViewModel("intro_01", "/Assets/UI/guilde_background.png"), game);
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Erreur base locale : {ex.Message}";
        }
        finally
        {
            NewGameButton.IsEnabled = true;
            ContinueButton.IsEnabled = true;
        }
    }

    private void OnContinueClicked(object sender, RoutedEventArgs e)
    {
        StatusText.Text = "Chargement de la dernière sauvegarde...";
   var saveSoloService = new SaveSoloService();
    var saves = saveSoloService.LoadAll();

    var latestSave = saves
        .Where(s => s.PlayerName == "test") // hardcoded for now, like OnNewGameClicked
        .OrderByDescending(s => s.LastPlayedAt)
        .FirstOrDefault();

    if (latestSave is null)
    {
        StatusText.Text = "Aucune sauvegarde trouvée.";
        return;
    }

    Game game = saveSoloService.LoadGame(latestSave.SaveId)!;
    NavigationService.CurrentSaveId = latestSave.SaveId;

    NavigationService.NavigateTo(new GuildViewModel(), game);
    }
}