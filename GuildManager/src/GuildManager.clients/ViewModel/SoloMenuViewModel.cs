using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.ViewModel;

public class SoloMenuViewModel : IScreenViewModel
{
    public string BackgroundPath => "/Assets/UI/background_menu.jpg";

    
    // Link to the save system: true if an existing game can be loaded.
    // Leave as true by default for now; the "Continue" button remains clickable.
    public bool HasSave { get; set; } = true;
}