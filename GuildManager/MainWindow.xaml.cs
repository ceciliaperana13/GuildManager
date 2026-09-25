using System.Windows;
using GuildManager.Client.ViewModel;
using GuildManager.Client.Services;
using GuildManager.Aplication.Guilds.Controls;

namespace MonProjet;

using GuildManager.Client.ViewModel;


using GuildManager.Client.ViewModel;


public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var mainWindowViewModel = new MainWindowViewModel();
        DataContext = mainWindowViewModel;
        DialogueRepository.Load();
        GuildManager.Client.Services.NavigationService.Initialize(mainWindowViewModel);       
        //NavigationService.NavigateTo(new DialogueViewModel("intro_01", "/Assets/UI/guilde_background.png"));
    }
}