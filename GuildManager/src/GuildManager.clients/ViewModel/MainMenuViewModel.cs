

using GuildManager.Client.ViewModel;

namespace GuildManager.Client.ViewModel;
public class MainMenuViewModel : IScreenViewModel
{
    public string BackgroundPath => "/Assets/UI/background_menu.jpg";
}

// ViewModel/CoopMenuViewModel.cs
public class CoopMenuViewModel : IScreenViewModel
{
    public string BackgroundPath => "/Assets/UI/background_menu.jpg"; // même fond que Menu/Solo
}