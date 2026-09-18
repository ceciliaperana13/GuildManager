using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace GuildManager.Client.View.Controls;
public partial class ScrollReveal : UserControl
{

    public static readonly DependencyProperty ScrollContentProperty = 
                        DependencyProperty.Register(nameof(ScrollContent), typeof(object), typeof(ScrollReveal) );

    public object ScrollContent
    {
        get => GetValue(ScrollContentProperty);
        set => SetValue(ScrollContentProperty, value);
    }

    public ScrollReveal()
    {
        InitializeComponent();
    }

    public void PlayOpenAnimation()
    {
        var storyboard = Resources["OuvrirParchemin"] as Storyboard
            ?? throw new InvalidOperationException("La ressource d'animation 'OuvrirParchemin' est introuvable.");
        storyboard.Begin(this);
    }
}