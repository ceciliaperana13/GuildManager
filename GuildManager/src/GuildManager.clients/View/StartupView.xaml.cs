using System;
using System.Windows;
using System.Windows.Controls;

namespace MonProjet.View;

public enum StartupMode
{
    Solo,
    Host,
    Guest
}

public partial class StartupView : UserControl
{
    public event Action<StartupMode, string>? ModeSelected; // mode, hostIp — vide si Solo

    public StartupView() => InitializeComponent();

    private void SoloButton_Click(object sender, RoutedEventArgs e)
        => ModeSelected?.Invoke(StartupMode.Solo, string.Empty);

    private void MultiplayerButton_Click(object sender, RoutedEventArgs e)
    {
        MainChoicePanel.Visibility = Visibility.Collapsed;
        MultiplayerChoicePanel.Visibility = Visibility.Visible;
    }
   
    private void Back_Click(object sender, RoutedEventArgs e)
    {
        MultiplayerChoicePanel.Visibility = Visibility.Collapsed;
        JoinPanel.Visibility = Visibility.Collapsed;
        MainChoicePanel.Visibility = Visibility.Visible;
        StatusText.Text = string.Empty;
    }

    private void HostButton_Click(object sender, RoutedEventArgs e)
    {
        JoinPanel.Visibility = Visibility.Collapsed;
        ModeSelected?.Invoke(StartupMode.Host, "localhost");
    }

    private void JoinButton_Click(object sender, RoutedEventArgs e)
        => JoinPanel.Visibility = Visibility.Visible;

    private void ConnectToHost_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(IpTextBox.Text))
        {
            MessageBox.Show("Merci de saisir l'adresse IP de l'hôte.");
            return;
        }
        ModeSelected?.Invoke(StartupMode.Guest, IpTextBox.Text.Trim());
    }

    public void SetStatus(string text) => StatusText.Text = text;
}