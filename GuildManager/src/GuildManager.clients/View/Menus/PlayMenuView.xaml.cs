using System;
using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.ViewModel;
using GuildManager.Client.Services;
using GuildManager.Infrastructure.Configurations;
using GuildManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.View;

public partial class PlayMenuView : UserControl
{
    public PlayMenuView()
    {
        InitializeComponent();

    }

    private async void OnSoloClicked(object sender, RoutedEventArgs e)
    {
        NavigationService.NavigateTo(new SoloMenuViewModel());
    }

    private void OnCoopClicked(object sender, RoutedEventArgs e)
    {
        // Vers le menu coop : hébergement d'une partie ou connexion à un hôte distant.
        // La création du Game et de l'AdventurerManager se fait côté serveur (CoopMenuView_Loaded).
        NavigationService.NavigateTo(new CoopMenuViewModel());
    }
}