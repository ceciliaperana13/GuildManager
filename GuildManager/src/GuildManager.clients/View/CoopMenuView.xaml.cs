using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.Services;
using GuildManager.Client.ViewModel;

namespace GuildManager.Client.View;

public partial class CoopMenuView : UserControl
{
    public CoopMenuView()
    {
        InitializeComponent();
    }

    private void ShowJoinPanel_Click(object sender, RoutedEventArgs e)
    {
        JoinPanel.Visibility = Visibility.Visible;
        HostIpTextBox.Focus();
    }

    // Héberger : l'API locale tourne déjà depuis le lancement de l'appli,
    private void HostButton_Click(object sender, RoutedEventArgs e)
    {
        AppSession.IsHost = true;
        StatusText.Text = $"Partie hébergée en tant que {AppSession.Username}.";
        // TODO (côté logique de jeu) : NavigationService.NavigateTo(new GameViewModel());
    }

        private async void JoinButton_Click(object sender, RoutedEventArgs e)
    {
        var hostIp = HostIpTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(hostIp))
        {
            StatusText.Text = "Saisissez l'adresse IP de l'hôte.";
            return;
        }

        var remoteUrl = $"http://{hostIp}:5080";
        StatusText.Text = "Vérification de l'hôte distant...";

        try
        {
            using var client = new HttpClient { BaseAddress = new Uri(remoteUrl), Timeout = TimeSpan.FromSeconds(5) };
            var response = await client.GetAsync("api/health");

            if (!response.IsSuccessStatusCode)
            {
                StatusText.Text = "Impossible de joindre cet hôte.";
                return;
            }
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Impossible de joindre l'hôte : {ex.Message}";
            return;
        }

        // Bascule vers la base de l'hôte distant : ré-authentification obligatoire
        AppSession.ApiBaseUrl = remoteUrl;
        AppSession.IsHost = false;
        AppSession.UserId = 0;
        AppSession.Username = string.Empty;

        StatusText.Text = "Connecté à l'hôte. Identifiez-vous sur cette partie.";
        NavigationService.NavigateTo(new AuthChoiceViewModel());
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
        => NavigationService.GoBack();
}