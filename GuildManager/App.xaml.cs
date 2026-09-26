using System;
using System.Runtime.InteropServices;
using System.Windows;
using GuildManager.Client.Services;

namespace MonProjet;

public partial class App : Application
{
    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        AllocConsole();
        Console.Title = "GuildManager - Logs";

        var mainWindow = new MainWindow();
        MainWindow = mainWindow;
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (NavigationService.CurrentGame is not null && NavigationService.CurrentSaveId is Guid saveId)
        {
            try
            {
                var saveSoloService = new SaveSoloService();
                saveSoloService.UpdateSave(saveId, NavigationService.CurrentGame);
                Console.WriteLine($"Save solo {saveId} sauvegardée à la fermeture.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la sauvegarde à la fermeture : {ex.Message}");
            }
        }

        base.OnExit(e);
    }
}