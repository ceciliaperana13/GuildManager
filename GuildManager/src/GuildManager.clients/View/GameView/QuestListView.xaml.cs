using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.ViewModel;
using GuildManager.Client.Models;
using GuildManager.Client.Services;
using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.View;

public partial class QuestListView : UserControl
{
    public QuestListView()
    {
        InitializeComponent();
    }

    private void OnQuestCardClicked(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is QuestCard quest)
        {
           NavigationService.NavigateTo(new QuestPreparationViewModel(quest));
        }
    }
}