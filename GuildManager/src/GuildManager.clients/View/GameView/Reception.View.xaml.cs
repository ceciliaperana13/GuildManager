using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.ViewModel;
using GuildManager.Client.Services;

namespace GuildManager.Client.View.GameView;

public partial class ReceptionView : UserControl
{
    public ReceptionView()
    {
        InitializeComponent();
    }

    private void OnMyAdventurersClicked(object sender, RoutedEventArgs e)
    {
        NavigationService.NavigateTo(new AdventurerRosterViewModel());
    }

    private void OnRecruitClicked(object sender, RoutedEventArgs e)
    {
     NavigationService.NavigateTo(new RecruitmentViewModel());
    }
}