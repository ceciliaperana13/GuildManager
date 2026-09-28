using System;
using GuildManager.Client.Services;

namespace GuildManager.Client.ViewModel;

public class SoloMenuViewModel : IScreenViewModel
{
    public string BackgroundPath => "/Assets/UI/background_menu.jpg";

    // true si savesolo.json contient au moins une sauvegarde : c'est ce qui
    public bool HasSave { get; }

    public SoloMenuViewModel()
    {
        try
        {
            HasSave = new SaveSoloService().LoadAll().Count > 0;
        }
        catch (Exception)
        {
            HasSave = false;
        }
    }
}