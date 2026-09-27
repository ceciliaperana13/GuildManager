using System.Windows.Controls;
using GuildManager.Client.Services;
using GuildManager.Client.ViewModel;
namespace GuildManager.Client.View;

public partial class VictoryView : UserControl
{
    public VictoryView() => InitializeComponent();

    private void OnReturnToMenuClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        NavigationService.NavigateTo(new SoloMenuViewModel());
    }
}