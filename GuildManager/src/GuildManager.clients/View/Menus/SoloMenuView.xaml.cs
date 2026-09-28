using System;
using System.Linq;
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
    // Évite un double clic pendant qu'une action est en cours.
    // On ne touche pas à ContinueButton.IsEnabled en code : il est piloté
    // par le binding "HasSave" du XAML, et l'écraser casserait ce binding.
    private bool _isBusy;

    public SoloMenuView()
    {
        InitializeComponent();
    }

    //  Nouvelle partie 

    private async void OnNewGameClicked(object sender, RoutedEventArgs e)
    {
        if (_isBusy) return;
        _isBusy = true;
        NewGameButton.IsEnabled = false;
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

            // Évite de rester en mode coop si on vient du menu coop
            AppSession.IsCoop = false;
            AppSession.IsHost = false;

            Game game = new Game("test", 1, 0, 10000, 10000, 1, 0);

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
            _isBusy = false;
        }
    }

    //  Continuer : charge la dernière save de savesolo.json 

    private void OnContinueClicked(object sender, RoutedEventArgs e)
    {
        if (_isBusy) return;

        try
        {
            StatusText.Text = "Chargement de la dernière sauvegarde...";

            var saveSoloService = new SaveSoloService();

            var latestSave = saveSoloService.LoadAll()
                .OrderByDescending(s => s.LastPlayedAt)
                .FirstOrDefault();

            if (latestSave is null)
            {
                StatusText.Text = "Aucune sauvegarde trouvée.";
                return;
            }

            Game? game = saveSoloService.LoadGame(latestSave.SaveId);
            if (game is null)
            {
                StatusText.Text = "La dernière sauvegarde est illisible.";
                return;
            }

            // On s'assure de ne pas rester en mode coop
            AppSession.IsCoop = false;
            AppSession.IsHost = false;

            // Les prochaines sauvegardes (UpdateSave) cibleront cette save-là
            NavigationService.CurrentSaveId = latestSave.SaveId;
            Console.WriteLine($"Save solo chargée : {latestSave.SaveId} (tour {latestSave.Turn})");

            NavigationService.NavigateTo(new GuildViewModel(), game);
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Erreur au chargement : {ex.Message}";
        }
    }
}