using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.ViewModel;
using GuildManager.Client.Services;

namespace GuildManager.Client.View;

public partial class GuildView : UserControl
{
    public GuildView()
    {
        InitializeComponent();
    }

    private void OnQuestBoardClicked(object sender, RoutedEventArgs e)
    {
        NavigationService.NavigateTo(new QuestListViewModel());
    }

    private void OnReceptionClicked(object sender, RoutedEventArgs e)
    {
        NavigationService.NavigateTo(new ReceptionViewModel());
    }
    private void OnStoreClicked(object sender, RoutedEventArgs e)
    {
    }
}