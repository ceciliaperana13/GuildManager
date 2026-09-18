using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
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
        LocalIpText.Text = $"Ton IP (à communiquer) : {GetLocalIpAddress()}";
    }

    private void ShowJoinPanel_Click(object sender, RoutedEventArgs e)
    {
        JoinPanel.Visibility = Visibility.Visible;
        HostIpTextBox.Focus();
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

    private static string GetLocalIpAddress()
    {
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ip = host.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
            return ip?.ToString() ?? "introuvable";
        }
        catch
        {
            return "introuvable";
        }
    }
}