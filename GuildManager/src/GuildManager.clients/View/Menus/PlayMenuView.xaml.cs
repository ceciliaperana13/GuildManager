using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.ViewModel;
using GuildManager.Client.Services;

namespace GuildManager.Client.View;

public partial class PlayMenuView : UserControl
{
    public PlayMenuView()
    {
        InitializeComponent();
    }

    // Solo : écran de choix Nouvelle partie / Continuer
    private void OnSoloClicked(object sender, RoutedEventArgs e)
    {
        NavigationService.NavigateTo(new SoloMenuViewModel());
    }

    // Coop : hébergement d'une partie ou connexion à un hôte distant
    private void OnCoopClicked(object sender, RoutedEventArgs e)
    {
        NavigationService.NavigateTo(new CoopMenuViewModel());
    }
}