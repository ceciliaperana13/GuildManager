
using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.ViewModel;
using GuildManager.Client.Services;

namespace GuildManager.Client.View;

public partial class LoginMenuView : UserControl
{
    public LoginMenuView()
    {
        InitializeComponent();
    }

    private void OnLoginClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        string username = UsernameBox.Text;
        string password = PasswordBox.Text;

        NavigationService.NavigateTo(new MainMenuViewModel());
    }

    private void OnGoToRegisterClicked(object sender, System.Windows.RoutedEventArgs e)
        => NavigationService.NavigateTo(new RegisterMenuViewModel());

     private void OnBackClicked(object sender, RoutedEventArgs e)
        => NavigationService.GoBack();
    
}