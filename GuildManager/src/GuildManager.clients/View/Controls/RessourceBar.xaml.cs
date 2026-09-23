using System.Windows;
using System.Windows.Controls;

namespace GuildManager.Client.View.Controls;

public partial class ResourceBar : UserControl
{
    public static readonly DependencyProperty GoldProperty =
        DependencyProperty.Register(nameof(Gold), typeof(int), typeof(ResourceBar));
    public static readonly DependencyProperty FoodProperty =
        DependencyProperty.Register(nameof(Food), typeof(int), typeof(ResourceBar));
    public static readonly DependencyProperty PrestigeProperty =
        DependencyProperty.Register(nameof(Prestige), typeof(int), typeof(ResourceBar));

    public int Gold { get => (int)GetValue(GoldProperty); set => SetValue(GoldProperty, value); }
    public int Food { get => (int)GetValue(FoodProperty); set => SetValue(FoodProperty, value); }
    public int Prestige { get => (int)GetValue(PrestigeProperty); set => SetValue(PrestigeProperty, value); }

    public ResourceBar()
    {
        InitializeComponent();
    }
}