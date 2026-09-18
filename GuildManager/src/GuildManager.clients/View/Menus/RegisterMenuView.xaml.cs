using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.ViewModel;
using GuildManager.Client.Services;

namespace GuildManager.Client.View;
public partial class RegisterMenuView : UserControl
{
    public RegisterMenuView()
    {
        InitializeComponent();
    }

    private void OnRegisterClicked(object sender, System.Windows.RoutedEventArgs e)
    {
        string username = UsernameBox.Text;
        string password = PasswordBox.Text;
        string confirmPassword = ConfirmPasswordBox.Text;
        NavigationService.NavigateTo(new MainMenuViewModel());
    }

    private void OnGoToLoginClicked(object sender, System.Windows.RoutedEventArgs e)
        => NavigationService.NavigateTo(new LoginMenuViewModel());

     private void OnBackClicked(object sender, RoutedEventArgs e)
        => NavigationService.GoBack();
    
}