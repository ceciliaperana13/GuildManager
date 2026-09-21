using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.ViewModel;
using GuildManager.Client.Services;


namespace GuildManager.Client.View;

public partial class MainMenuView : UserControl
{
    public MainMenuView()
    {
        InitializeComponent();
    }

    private void OnPlayButtonClicked(object sender, RoutedEventArgs e)
    {
        // vers le menu choix solo ou coop
        NavigationService.NavigateTo(new PlayMenuViewModel());
    }

    private void OnOptionsButtonClicked(object sender, RoutedEventArgs e)
    {
        // Logique pour ouvrir les options du jeu
        MessageBox.Show("Options button clicked!");
    }

    private void OnQuitButtonClicked(object sender, RoutedEventArgs e)
    {
        // Logique pour quitter l'application
        Application.Current.Shutdown();
    }
}