using System.Windows.Controls;
using GuildManager.Client.ViewModel;

namespace GuildManager.Client.View;

public partial class AdventurerRosterView : UserControl
{
    public AdventurerRosterView()
    {
        InitializeComponent();
        DataContext = new AdventurerRosterViewModel();
    }
}