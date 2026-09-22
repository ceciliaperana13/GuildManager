
using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.ViewModel;
using GuildManager.Client.View.Controls;
using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.View;


public partial class RecruitmentView : UserControl
{
    public RecruitmentView(Game game)
    {
        InitializeComponent();
        DataContext = new RecruitmentViewModel(game); // à modifier avec le préstige
    }

     private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Scroll.PlayOpenAnimation();
    }


}