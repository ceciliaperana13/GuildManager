using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GuildManager.Client.View.Controls;

public partial class FantasyMenuButton :UserControl
{
   public static readonly DependencyProperty NormalImageProperty = DependencyProperty.Register(
    nameof(NormalImage), typeof(string), typeof(FantasyMenuButton), new PropertyMetadata(null,OnImageChanged));
   
   public static readonly DependencyProperty HoverImageProperty = DependencyProperty.Register(
    nameof(HoverImage), typeof(string), typeof(FantasyMenuButton));

    public string NormalImage
    {
        get => (string)GetValue(NormalImageProperty);
        set => SetValue(NormalImageProperty, value);
    }

    public string HoverImage
    {
        get => (string)GetValue(HoverImageProperty);
        set => SetValue(HoverImageProperty, value);
    }

    public event RoutedEventHandler? Clicked;

    public FantasyMenuButton()
    {
        InitializeComponent();
        MouseEnter += (s,e) => SetImage(HoverImage);
        MouseLeave += (s,e) => SetImage(NormalImage);
    }

    private static void OnImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FantasyMenuButton btn) btn.SetImage(e.NewValue as string);
    }

    private void SetImage(string? imagePath)
    {
        if (string.IsNullOrEmpty(imagePath)) return;
        var uri = new Uri("pack://application:,,,/" + imagePath.TrimStart('/'), UriKind.Absolute);
        ButtonImage.Source = new BitmapImage(uri);
    }

    private void Button_Click(object sender, RoutedEventArgs e) => Clicked?.Invoke(this, e);
 
}