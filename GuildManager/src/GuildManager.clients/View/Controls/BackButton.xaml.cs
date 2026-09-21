using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using GuildManager.Client.Services;

namespace GuildManager.Client.View.Controls;

public partial class BackButton : UserControl
{
    public BackButton()
    {
        InitializeComponent();
        MouseEnter += (s, e) => SetImage("/Assets/UI/Icon_retour_hover.png");
        MouseLeave += (s, e) => SetImage("/Assets/UI/Icon_retour.png");
    }

    private void SetImage(string path)
    {
        var uri = new Uri("pack://application:,,,/" + path.TrimStart('/'), UriKind.Absolute);
        ButtonImage.Source = new BitmapImage(uri);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
        => NavigationService.GoBack();
}