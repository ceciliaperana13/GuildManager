using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.ViewModel;
using GuildManager.Client.Services;
using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.View.GameView;

public partial class ReceptionView : UserControl
{
    public ReceptionView()
    {
        InitializeComponent();
    }

    private Game? Game => (DataContext as ReceptionViewModel)?.Game;

    private void OnMyAdventurersClicked(object sender, RoutedEventArgs e)
    {
        if (Game is null) return; // sécurité si DataContext pas encore prêt
        NavigationService.NavigateTo(new AdventurerRosterViewModel(Game), Game);
    }

    private void OnRecruitClicked(object sender, RoutedEventArgs e)
    {
        if (Game is null) return;
        NavigationService.NavigateTo(new RecruitmentViewModel(Game));
    }
}