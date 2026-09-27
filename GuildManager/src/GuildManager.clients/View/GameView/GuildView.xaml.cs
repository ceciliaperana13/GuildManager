using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.ViewModel;
using GuildManager.Client.Models;
using GuildManager.Client.Services;
using GuildManager.Client.Models;
using GuildManager.Client.View.Controls;
using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.View;

public partial class GuildView : UserControl
{
    private System.Timers.Timer? _refreshTimer;
    private Game? Game => (DataContext as GuildViewModel)?.Game;

    private QuestSummary? _currentSummaryPopup;

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

    private async void OnFlipClicked(object sender, RoutedEventArgs e)
    {
        if (Game is null) return;

        int goldBefore = Game.gold;
        int foodBefore = Game.food;

        Game.passTurn();

        // En coop, l'or/la nourriture gagnés ou consommés pendant ce tour doivent
        // être répercutés sur le pot commun partagé (saves.json côté serveur),
        // pour que tous les joueurs voient le même solde à jour.
        if (Game.IsCoop)
        {
            int goldDelta = Game.gold - goldBefore;
            int foodDelta = Game.food - foodBefore;

            if (goldDelta != 0 || foodDelta != 0)
            {
                var apiClient = new GuildApiClient();
                var (success, resources, _) = await apiClient.AdjustResourcesAsync(goldDelta, foodDelta);
                if (success)
                    Game.SyncResources(resources!.Gold, resources.Food);
            }
        }
        else if (NavigationService.CurrentSaveId is Guid saveId)
        {
            // Solo : la progression (or, nourriture, aventuriers, xp, quêtes...)
            // est persistée localement dans savesolo.json après chaque tour.
            new SaveSoloService().UpdateSave(saveId, Game);
        }

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

        var pendingSummaries = new Queue<QuestSummaryData>(Game.LastQuestSummaries);

        foreach (var data in DequeueAll(pendingSummaries))
        {
            await PlayCoinFlipAsync();
            await ShowSummaryAsync(data);
        }
    }

    private static IEnumerable<QuestSummaryData> DequeueAll(Queue<QuestSummaryData> queue)
    {
        while (queue.Count > 0)
            yield return queue.Dequeue();
    }

    private Task PlayCoinFlipAsync()
    {
        var tcs = new TaskCompletionSource();

        GuildCoinFlip.Visibility = Visibility.Visible;

        void OnFinished(object? sender, EventArgs e)
        {
            GuildCoinFlip.FlipCompleted -= OnFinished;
            tcs.SetResult();
        }

        GuildCoinFlip.FlipCompleted += OnFinished;
        GuildCoinFlip.PlayFlip(Random.Shared.Next(2) == 1);

        return tcs.Task;
    }

    private Task ShowSummaryAsync(QuestSummaryData data)
    {
        var tcs = new TaskCompletionSource();

        var popup = new QuestSummary
        {
            Width = RootCanvas.Width,
            Height = RootCanvas.Height
        };
        popup.SetData(data);

        Canvas.SetLeft(popup, 0);
        Canvas.SetTop(popup, 0);

        void OnClosed()
        {
            popup.CancelRequested -= OnClosed;
            RootCanvas.Children.Remove(popup);
            _currentSummaryPopup = null;
            tcs.SetResult();
        }

        popup.CancelRequested += OnClosed;

        _currentSummaryPopup = popup;
        RootCanvas.Children.Add(popup);

        return tcs.Task;
    }
}