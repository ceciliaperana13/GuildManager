
using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.ViewModel;
using GuildManager.Client.View.Controls;

namespace GuildManager.Client.View;


public partial class RecruitmentView : UserControl
{
    public RecruitmentView()
    {
        InitializeComponent();
        DataContext = new RecruitmentViewModel();
    }

     private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Scroll.PlayOpenAnimation();
    }
}