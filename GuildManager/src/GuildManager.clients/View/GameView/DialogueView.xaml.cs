using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using GuildManager.Client.ViewModel;
using GuildManager.Client.Models;

namespace GuildManager.Client.View;

public partial class DialogueView : UserControl
{
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
}