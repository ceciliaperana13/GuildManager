using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GuildManager.Client.Models; 

namespace GuildManager.Client.View.Controls;

public partial class TextPopup : UserControl
{
    public event Action? CancelRequested;

    public TextPopup()
    {
        InitializeComponent();
    }

    public void SetText(string message)
    {
        MessageText.Text = message;
    }

    private void OnBackgroundClicked(object sender, MouseButtonEventArgs e) => CancelRequested?.Invoke();
    // la fonction s'execute mais l'invoke ne marche pas
    private void OnPanelClicked(object sender, MouseButtonEventArgs e) => e.Handled = true; // évite de fermer en cliquant le panneau
}