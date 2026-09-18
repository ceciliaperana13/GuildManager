using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.ViewModel;
using GuildManager.Client.Services;

namespace GuildManager.Client.View;

public partial class AuthChoiceView : UserControl
{
    public AuthChoiceView()
    {
        InitializeComponent();
    }

    private void OnLoginClicked(object sender, RoutedEventArgs e)
        => NavigationService.NavigateTo(new LoginMenuViewModel());

    private void OnRegisterClicked(object sender, RoutedEventArgs e)
        => NavigationService.NavigateTo(new RegisterMenuViewModel());

    private void OnBackClicked(object sender, RoutedEventArgs e)
        => NavigationService.GoBack();
}