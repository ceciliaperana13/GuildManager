using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.Services;
using GuildManager.Client.ViewModel;
using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.View;

public partial class LoginMenuView : UserControl
{
    public LoginMenuView()
    {
        InitializeComponent();
    }

    private record LoginResult(int Id, string Username, string Email);

    private async void OnLoginClicked(object sender, RoutedEventArgs e)
    {
        var username = UsernameBox.Text.Trim();
        var password = PasswordBox.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            StatusText.Text = "Renseignez le nom d'utilisateur et le mot de passe.";
            return;
        }

        if (string.IsNullOrWhiteSpace(AppSession.ApiBaseUrl))
        {
            StatusText.Text = "Aucune connexion au serveur. Revenez à l'étape Héberger/Rejoindre.";
            return;
        }

        StatusText.Text = "Connexion en cours...";

        try
        {
            using var client = new HttpClient { BaseAddress = new Uri(AppSession.ApiBaseUrl) };
            var response = await client.PostAsJsonAsync("api/auth/login",
                new { Username = username, Password = password });

            if (!response.IsSuccessStatusCode)
            {
                StatusText.Text = "Identifiants invalides.";
                return;
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResult>();

            AppSession.UserId = result!.Id;
            AppSession.Username = result.Username;

            // Enregistrement + heartbeat en continu, tant que l'appli reste ouverte
            SessionKeepAlive.Start(AppSession.ApiBaseUrl, AppSession.Username);
            Game game = new Game("test", 1, 0, 10000, 10000, 1, 0);
            NavigationService.NavigateTo(new GuildViewModel(), game);
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Erreur de connexion : {ex.Message}";
        }
    }

    private void OnGoToRegisterClicked(object sender, RoutedEventArgs e)
        => NavigationService.NavigateTo(new RegisterMenuViewModel());

    private void OnBackClicked(object sender, RoutedEventArgs e)
        => NavigationService.GoBack();
}