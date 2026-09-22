using System.Windows;
using GuildManager.Client.ViewModel;
using GuildManager.Client.Services;
using GuildManager.Aplication.Guilds.Controls;

namespace MonProjet;

using GuildManager.Client.ViewModel;


public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var mainWindowViewModel = new MainWindowViewModel();
        DataContext = mainWindowViewModel;
        GuildManager.Client.Services.NavigationService.Initialize(mainWindowViewModel);
    }
}