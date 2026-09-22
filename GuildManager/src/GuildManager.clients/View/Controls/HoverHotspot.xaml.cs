using System.Windows;
using System.Windows.Controls;

namespace GuildManager.Client.View.Controls
{
    public partial class HoverHotspot : UserControl
    {
        public event RoutedEventHandler? Clicked;

        public HoverHotspot()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) => Clicked?.Invoke(sender, e);
    }
}