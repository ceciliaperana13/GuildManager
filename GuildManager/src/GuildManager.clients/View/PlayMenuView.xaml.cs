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

        private void OnSoloClicked(object sender, RoutedEventArgs e)
    {
        // navigation vers l'écran Guild en mode solo
    }

    private void OnCoopClicked(object sender, RoutedEventArgs e)
    {
        // navigation vers l'écran Coop (adresse du salon)
        NavigationService.NavigateTo(new CoopMenuViewModel());
    }
}