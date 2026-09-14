namespace GuildManager.Client.Services;

using GuildManager.Client.ViewModel;

public static class NavigationService
{
   private static MainWindowViewModel? _mainWindowViewModel;

   public static void Initialize(MainWindowViewModel mainWindowViewModel)=> _mainWindowViewModel = mainWindowViewModel;
    public static void NavigateTo(IScreenViewModel viewModel)=> _mainWindowViewModel?.NavigateTo(viewModel);
   
}