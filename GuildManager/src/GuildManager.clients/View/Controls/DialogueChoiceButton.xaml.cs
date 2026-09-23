using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GuildManager.Client.View.Controls;

public partial class DialogueChoiceButton : UserControl
{
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(DialogueChoiceButton));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public event RoutedEventHandler? Clicked;

    public DialogueChoiceButton()
    {
        InitializeComponent();
        MouseEnter += (s, e) => SetImage("/Assets/UI/choice_box_hover.png");
        MouseLeave += (s, e) => SetImage("/Assets/UI/choice_box.png");
    }

    private void SetImage(string path)
    {
        var uri = new System.Uri("pack://application:,,,/" + path.TrimStart('/'), System.UriKind.Absolute);
        ButtonImage.Source = new BitmapImage(uri);
    }

    private void Button_Click(object sender, RoutedEventArgs e) => Clicked?.Invoke(this, e);
}