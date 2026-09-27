using System.Windows.Controls;
using GuildManager.Client.Services;
using GuildManager.Client.ViewModel;
namespace GuildManager.Client.View;

public partial class DefeatView : UserControl
{
    public DefeatView() => InitializeComponent();

    private void OnReturnToMenuClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        NavigationService.NavigateTo(new SoloMenuViewModel());
    }
}