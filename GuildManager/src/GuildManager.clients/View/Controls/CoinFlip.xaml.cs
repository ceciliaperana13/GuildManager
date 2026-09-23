using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Threading;


namespace GuildManager.Client.View.Controls;

public partial class CoinFlip : UserControl
{
    private DispatcherTimer? _hideTimer;

    public CoinFlip()
    {
        InitializeComponent();
    }

    public void PlayFlip(bool victory)
    {
        _hideTimer?.Stop();

        var animationKey = victory ? "VictoryFlip" : "DefeatFlip";
        var animation = (Storyboard)Resources[animationKey];

        CoinSide.Opacity = 1;
        CoinSide.RenderTransform = new ScaleTransform(1, 1);
        CoinVictory.Opacity = 0;
        CoinVictory.RenderTransform = new ScaleTransform(0.08, 1);
        CoinDefeat.Opacity = 0;
        CoinDefeat.RenderTransform = new ScaleTransform(0.08, 1);

        animation.Begin(this, true);
    }

    private void OnFlipCompleted(object? sender, EventArgs e)
    {
        _hideTimer?.Stop();
        _hideTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
        _hideTimer.Tick += HideCoin;
        _hideTimer.Start();
    }

    private void HideCoin(object? sender, EventArgs e)
    {
        _hideTimer?.Stop();
        Visibility = Visibility.Collapsed;
    }
}