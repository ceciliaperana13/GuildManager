using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.ViewModel;
using GuildManager.Client.Models;
using GuildManager.Client.Services;
using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.View;

public partial class GuildView : UserControl
{
    private System.Timers.Timer? _refreshTimer;
    private Game? Game => (DataContext as GuildViewModel)?.Game;

    public GuildView()
    {
        InitializeComponent();
        Loaded += GuildView_Loaded;
        Unloaded += GuildView_Unloaded;
    }

    private async void GuildView_Loaded(object sender, RoutedEventArgs e)
    {
        await RefreshOnlinePlayersAsync();

        _refreshTimer = new System.Timers.Timer(5000);
        _refreshTimer.Elapsed += async (_, _) => await RefreshOnlinePlayersAsync();
        _refreshTimer.Start();
    }

    private void GuildView_Unloaded(object sender, RoutedEventArgs e)
    {
        _refreshTimer?.Stop();
        _refreshTimer?.Dispose();
        _refreshTimer = null;
    }

    private async System.Threading.Tasks.Task RefreshOnlinePlayersAsync()
    {
        var players = await SessionKeepAlive.GetOnlinePlayersAsync();
        var names = players.Select(p => p.Name == AppSession.Username ? $"{p.Name} (vous)" : p.Name).ToList();

        Dispatcher.Invoke(() => OnlinePlayersList.ItemsSource = names);
    }

    private void OnQuestBoardClicked(object sender, RoutedEventArgs e)
    {
        if (Game is null) return;
        NavigationService.NavigateTo(new QuestListViewModel(Game));
    }

    private void OnReceptionClicked(object sender, RoutedEventArgs e)
    {
        NavigationService.NavigateTo(new ReceptionViewModel());
    }

    private void OnStoreClicked(object sender, RoutedEventArgs e)
    {
    }

    private void OnFlipClicked(object sender, RoutedEventArgs e)
{
    if (Game is null) return;

    Game.passTurn();
    (DataContext as GuildViewModel)?.RefreshResources();

    DialogueEntry? dialogue = null;
    if (Game.LastQuestSucceeded is not null)
{
    dialogue = DialogueRepository.GetByTrigger(
        "questResult",
        Game.mainProgress,
        Game.HasStoryFlag,
        Game.ShownDialogueIds,
        Game.LastQuestSucceeded,
        Game.LastCompletedStoryDialogueId);
}
    if (dialogue is null && Game.IsPeriodicDialogueTurn)
    {
        dialogue = DialogueRepository.GetByTrigger(
            "manual",
            Game.mainProgress,
            Game.HasStoryFlag,
            Game.ShownDialogueIds);
    }

    if (dialogue is null && Game.LastQuestSucceeded is null)
    {
        dialogue = DialogueRepository.GetByTrigger(
            "turnStart",
            Game.mainProgress,
            Game.HasStoryFlag,
            Game.ShownDialogueIds);
    }
    if (dialogue is not null)
    {
        if (dialogue.Trigger == "questResult")
            Game.MarkDialogueAsShown(dialogue.Id);

        NavigationService.NavigateTo(
            new DialogueViewModel(dialogue.Id, "/Assets/UI/guilde_background.png"),
            Game);
        return;
    }

    GuildCoinFlip.Visibility = Visibility.Visible;
    GuildCoinFlip.PlayFlip(Random.Shared.Next(2) == 1);
}
}