using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.ViewModel;

public class SoloMenuViewModel : IScreenViewModel
{
    public string BackgroundPath => "/Assets/UI/background_menu.jpg";

    // À relier par le système de sauvegarde : true si une partie existante peut être chargée.
    // Laisser à true par défaut pour l'instant ; le bouton "Continuer" reste cliquable.
    public bool HasSave { get; set; } = true;
}