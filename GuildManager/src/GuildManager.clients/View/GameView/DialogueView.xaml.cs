using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using GuildManager.Client.ViewModel;
using GuildManager.Client.Models;
using GuildManager.Client.Services;
using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.View;

public partial class DialogueView : UserControl
{
    private Game? game => (DataContext as DialogueViewModel)?.Game;
    public DialogueView()
    {
        InitializeComponent();

    }



    private void OnAdvanceClicked(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is DialogueViewModel vm) vm.Advance();
    }

    private void OnChoiceClicked(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is DialogueChoice choice
            && DataContext is DialogueViewModel vm)
        {
            vm.ChooseOption(choice);
        }
    }

    private void OnEndDialogueClicked(object sender, MouseButtonEventArgs e)
    {
        if (game is null)
        {
            NavigationService.NavigateTo(new GuildViewModel());
            return;
        }

        NavigationService.NavigateTo(new GuildViewModel(), game);
    }
}