using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;

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

        // Capture toutes les exceptions non gérées pour qu'elles s'affichent au lieu de fermer silencieusement
        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            Console.WriteLine("=== CRASH (UnhandledException) ===");
            Console.WriteLine(args.ExceptionObject.ToString());
            Console.WriteLine("Appuie sur une touche pour fermer...");
            Console.ReadKey();
        };

        DispatcherUnhandledException += (s, args) =>
        {
            Console.WriteLine("=== CRASH (DispatcherUnhandledException) ===");
            Console.WriteLine(args.Exception.ToString());
            MessageBox.Show(args.Exception.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = true; // empêche la fermeture immédiate pour que tu puisses lire
        };

        try
        {
            var mainWindow = new MainWindow();
            MainWindow = mainWindow;
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            Console.WriteLine("=== CRASH AU DÉMARRAGE ===");
            Console.WriteLine(ex.ToString());
            Console.WriteLine("Appuie sur une touche pour fermer...");
            Console.ReadKey();
        }
    }
}