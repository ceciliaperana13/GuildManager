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

    private void OnMyAdventurersClicked(object sender, RoutedEventArgs e, Game game)
    {
        NavigationService.NavigateTo(new AdventurerRosterViewModel(game), game);
    }

    private void OnRecruitClicked(object sender, RoutedEventArgs e, Game game)
    {
        NavigationService.NavigateTo(new RecruitmentViewModel(game));
    }
}