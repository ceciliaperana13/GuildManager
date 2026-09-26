using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GuildManager.Client.Models;

namespace GuildManager.Client.View.Controls;

public partial class QuestSummary : UserControl
{
    public event Action? CancelRequested;

    public int XpPerAdventurer { get; private set; }

    public QuestSummary()
    {
        InitializeComponent();
    }

    public void SetData(QuestSummaryData data)
    {
        QuestNameText.Text = data.QuestName;

        ResultText.Text = data.Victory ? "Victoire !" : "Défaite";
        ResultText.Foreground = (Brush)new BrushConverter().ConvertFromString(data.Victory ? "#2E7D32" : "#C62828");

        XpPerAdventurer = data.XpPerAdventurer;
        AdventurersList.ItemsSource = data.Adventurers;

        GoldText.Text = $"💰 {data.Reward.gold} or";
        FoodText.Text = $"🍖 {data.Reward.food} nourriture";
        PrestigeText.Text = $"⭐ {data.Reward.prestige} prestige";
    }

    private void OnBackgroundClicked(object sender, MouseButtonEventArgs e) => CancelRequested?.Invoke();
    private void OnPanelClicked(object sender, MouseButtonEventArgs e) => e.Handled = true;
}