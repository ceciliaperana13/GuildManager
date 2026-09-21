using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.ViewModel;
using GuildManager.Client.Models;
using GuildManager.Client.Services;

namespace GuildManager.Client.View;

public partial class QuestListView : UserControl
{
    public QuestListView()
    {
        InitializeComponent();
        DataContext = new QuestListViewModel();
    }

    private void OnQuestCardClicked(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is Quest quest)
        {
           NavigationService.NavigateTo(new QuestPreparationViewModel(quest));
        }
    }
}