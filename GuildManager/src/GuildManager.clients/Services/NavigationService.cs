namespace GuildManager.Client.Services;

using GuildManager.Aplication.Guilds.Controls;
using GuildManager.Client.ViewModel;
using System;
using System.Collections.Generic;

public static class NavigationService
{
    private static MainWindowViewModel? _mainWindowViewModel;
    private static readonly Stack<IScreenViewModel> _history = new();

    // État de la partie en cours, accessible depuis n'importe où (App.xaml.cs inclus)
    public static Game? CurrentGame { get; private set; }
    public static Guid? CurrentSaveId { get; set; }

    public static void Initialize(MainWindowViewModel mainWindowViewModel) => _mainWindowViewModel = mainWindowViewModel;

    public static void NavigateTo(IScreenViewModel viewModel, Game game)
    {
        if (_mainWindowViewModel?.CurrentView is IScreenViewModel current)
            _history.Push(current);

        CurrentGame = game;
        _mainWindowViewModel?.NavigateTo(viewModel, game);
    }

    public static void NavigateTo(IScreenViewModel viewModel)
    {
        if (_mainWindowViewModel?.CurrentView is IScreenViewModel current)
            _history.Push(current);

        _mainWindowViewModel?.NavigateTo(viewModel);
    }

    public static void GoBack()
    {
        if (_history.Count > 0)
            _mainWindowViewModel?.NavigateTo(_history.Pop());
    }
}