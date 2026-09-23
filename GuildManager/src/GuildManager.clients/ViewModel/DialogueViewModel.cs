using System.ComponentModel;
using GuildManager.Client.Models;
using GuildManager.Client.Services;

namespace GuildManager.Client.ViewModel;

public class DialogueViewModel : IScreenViewModel, INotifyPropertyChanged
{
    private DialogueEntry _entry;
    private int _lineIndex;

    public string BackgroundPath { get; private set; }

    public string CurrentText => _entry.Lines[_lineIndex];
    public string Speaker => _entry.Speaker;
    public bool IsLastLine => _lineIndex == _entry.Lines.Count - 1;
    public bool HasChoices => IsLastLine && _entry.Choices is { Count: > 0 };
    public List<DialogueChoice>? CurrentChoices => HasChoices ? _entry.Choices : null;


    public event PropertyChangedEventHandler? PropertyChanged;

    public DialogueViewModel(string dialogueId, string backgroundPath)
    {
        _entry = DialogueRepository.Get(dialogueId);
        BackgroundPath = backgroundPath;
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
            // TODO: écran de fin de partie (Win/Game over)
        }
        else
        {
            NavigationService.NavigateTo(new GuildViewModel());
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
            // TODO: écran de fin de partie
        }
        else
        {
            NavigationService.NavigateTo(new GuildViewModel());
        }
    }

    private void Raise() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
}