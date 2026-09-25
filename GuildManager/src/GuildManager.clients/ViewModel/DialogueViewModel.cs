using System.ComponentModel;
using GuildManager.Aplication.Guilds.Controls;
using GuildManager.Client.Models;
using GuildManager.Client.Services;

namespace GuildManager.Client.ViewModel;

public class DialogueViewModel : IScreenViewModel, IGameAwareViewModel, INotifyPropertyChanged
{
    private DialogueEntry _entry;
    private int _lineIndex;
    private Game _game = null!;

    public string BackgroundPath { get; private set; }
    public Game Game
    {
        get => _game;
        private set { _game = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Game))); }
    }
    public string CurrentText => _entry.Lines[_lineIndex];
    public string Speaker => _entry.Speaker;
    public bool IsLastLine => _lineIndex == _entry.Lines.Count - 1;
    public bool HasChoices => IsLastLine && _entry.Choices is { Count: > 0 };
    public List<DialogueChoice>? CurrentChoices => HasChoices ? _entry.Choices : null;


    public event PropertyChangedEventHandler? PropertyChanged;

    public string? CharacterImagePath =>
        _entry.Image != null ? "/Assets/" + _entry.Image.Replace('\\', '/') : null;

    public DialogueViewModel(string dialogueId, string backgroundPath)
    {
        _entry = DialogueRepository.Get(dialogueId);
        BackgroundPath = backgroundPath;
    }

    public void SetGame(Game game)
    {
        Game = game;
    }

    public void Advance()
    {
        if (HasChoices) return;

        if (!IsLastLine)
        {
            _lineIndex++;
            Raise();
        }
        else if (_entry.EndsGame)
        {
            ReturnToGameMenu();
        }
        else
        {
            ReturnToGameMenu();
        }
    }

    public void ChooseOption(DialogueChoice choice)
    {
        // TODO: appliquer choice.Effects via la logique de jeu 

        if (choice.NextDialogueId != null)
        {
            _entry = DialogueRepository.Get(choice.NextDialogueId);
            _lineIndex = 0;
            Raise();
        }
        else if (choice.EndsGame == true)
        {
            ReturnToGameMenu();
        }
        else
        {
            ReturnToGameMenu();
        }
    }

    private void ReturnToGameMenu() => NavigationService.NavigateTo(new GuildViewModel(), Game);


    private void Raise() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
}