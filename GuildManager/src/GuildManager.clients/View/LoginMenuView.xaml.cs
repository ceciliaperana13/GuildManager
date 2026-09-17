using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.Services;
using GuildManager.Client.ViewModel;

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

            // Enregistrement dans le tracker de connexions (pseudo réel, pas le nom de la machine)
            try
            {
                await client.PostAsJsonAsync("api/session/hello",
                    new { PlayerName = AppSession.Username });
            }
            catch
            {
                // Le tracker est optionnel : un échec ne doit pas bloquer l'entrée en jeu.
            }

            NavigationService.NavigateTo(new MainMenuViewModel());
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