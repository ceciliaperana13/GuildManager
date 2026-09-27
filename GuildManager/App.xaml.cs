using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace MonProjet;
//log
public partial class App : Application
{
    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        AllocConsole();
        Console.Title = "GuildManager - Logs";
                // Capture les exceptions non gérées sur le thread UI (WPF)
        this.DispatcherUnhandledException += (s, args) =>
        {
            Console.WriteLine("=== EXCEPTION NON GÉRÉE (UI) ===");
            Console.WriteLine(args.Exception.ToString());
            Console.WriteLine("\nAppuyez sur une touche pour fermer...");
            Console.ReadKey();
            args.Handled = true; // empêche le crash immédiat, l'app reste ouverte
        };

        // Filet de sécurité pour les exceptions hors thread UI
        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            Console.WriteLine("=== EXCEPTION NON GÉRÉE (hors UI) ===");
            Console.WriteLine(args.ExceptionObject.ToString());
            Console.WriteLine("\nAppuyez sur une touche pour fermer...");
            Console.ReadKey();
        };

        var mainWindow = new MainWindow();
        MainWindow = mainWindow;
        mainWindow.Show();
    }
}