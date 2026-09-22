using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.ViewModel;
using GuildManager.Client.Services;

namespace GuildManager.Client.View;

public partial class GuildView : UserControl
{
    private System.Timers.Timer? _refreshTimer;

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
        NavigationService.NavigateTo(new QuestListViewModel());
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
        GuildCoinFlip.Visibility = Visibility.Visible;
        GuildCoinFlip.PlayFlip(Random.Shared.Next(2) == 1);
        //Next tour
    }
}