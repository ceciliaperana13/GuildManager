using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GuildManager.Client.Models; 

namespace GuildManager.Client.View.Controls;

public partial class AdventurerPickerPopup : UserControl
{
    public event Action<Adventurer>? AdventurerPicked;
    public event Action? CancelRequested;

    public AdventurerPickerPopup()
    {
        InitializeComponent();
    }

    public void SetAdventurers(IEnumerable<Adventurer> adventurers)
    {
        AdventurerList.ItemsSource = adventurers;
    }

    private void OnAdventurerRowClicked(object sender, MouseButtonEventArgs e)
    {
        if (((FrameworkElement)sender).Tag is Adventurer adventurer)
            AdventurerPicked?.Invoke(adventurer);
    }

    private void OnBackgroundClicked(object sender, MouseButtonEventArgs e) => CancelRequested?.Invoke();
    private void OnPanelClicked(object sender, MouseButtonEventArgs e) => e.Handled = true; // évite de fermer en cliquant le panneau
}