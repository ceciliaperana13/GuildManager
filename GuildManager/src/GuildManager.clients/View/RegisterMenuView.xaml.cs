using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.Services;
using GuildManager.Client.ViewModel;

namespace GuildManager.Client.View;

public partial class RegisterMenuView : UserControl
{
    public RegisterMenuView()
    {
        InitializeComponent();
    }

    private record RegisterResult(int Id, string Username, string Email);

    private async void OnRegisterClicked(object sender, RoutedEventArgs e)
    {
        var username = UsernameBox.Text.Trim();
        var email = EmailBox.Text.Trim();
        var password = PasswordBox.Text;
        var confirmPassword = ConfirmPasswordBox.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            StatusText.Text = "Remplissez tous les champs.";
            return;
        }

        if (password != confirmPassword)
        {
            StatusText.Text = "Les mots de passe ne correspondent pas.";
            return;
        }

        if (string.IsNullOrWhiteSpace(AppSession.ApiBaseUrl))
        {
            StatusText.Text = "Aucune connexion au serveur. Revenez à l'étape Héberger/Rejoindre.";
            return;
        }

        StatusText.Text = "Inscription en cours...";

        try
        {
            using var client = new HttpClient { BaseAddress = new Uri(AppSession.ApiBaseUrl) };
            var response = await client.PostAsJsonAsync("api/auth/register",
                new { Username = username, Email = email, Password = password });

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                StatusText.Text = response.StatusCode == System.Net.HttpStatusCode.Conflict
                    ? "Ce nom d'utilisateur ou cet email est déjà utilisé."
                    : $"Échec de l'inscription : {error}";
                return;
            }

            var result = await response.Content.ReadFromJsonAsync<RegisterResult>();

            AppSession.UserId = result!.Id;
            AppSession.Username = result.Username;

            // Enregistrement + heartbeat en continu, tant que l'appli reste ouverte
            SessionKeepAlive.Start(AppSession.ApiBaseUrl, AppSession.Username);

            NavigationService.NavigateTo(new GuildViewModel());
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Erreur de connexion : {ex.Message}";
        }
    }

    private void OnGoToLoginClicked(object sender, RoutedEventArgs e)
        => NavigationService.NavigateTo(new LoginMenuViewModel());

    private void OnBackClicked(object sender, RoutedEventArgs e)
        => NavigationService.GoBack();
}