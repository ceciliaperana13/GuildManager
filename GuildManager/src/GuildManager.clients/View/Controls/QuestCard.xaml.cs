using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GuildManager.Client.View.Controls;

public partial class QuestCard : UserControl
{
    public event RoutedEventHandler? Clicked;

    public QuestCard()
    {
        InitializeComponent();
        MouseEnter += (s, e) => SetImage("/Assets/UI/quest_icon_hover.png");
        MouseLeave += (s, e) => SetImage("/Assets/UI/quest_icon.png");
    }

    private void SetImage(string path)
    {
        var uri = new Uri("pack://application:,,,/" + path.TrimStart('/'), UriKind.Absolute);
        CardImage.Source = new BitmapImage(uri);
    }

    private void Button_Click(object sender, RoutedEventArgs e) => Clicked?.Invoke(this, e);
}