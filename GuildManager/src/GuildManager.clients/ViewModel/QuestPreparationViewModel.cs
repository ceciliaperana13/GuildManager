using System.Collections.ObjectModel;
using System.Linq;
using GuildManager.Client.Models;

namespace GuildManager.Client.ViewModel;

public class QuestPreparationViewModel : IScreenViewModel
{
    public string BackgroundPath => "/Assets/UI/quest_background.png";
    public QuestCard? SelectedQuest { get; }
    public string BackgroundPath => "/Assets/UI/Quest_background.png";
    public Quest? SelectedQuest { get; }

    public ObservableCollection<AdventurerCard?> SelectedSlots { get; } = new() { null, null, null };

    private int? _successPercentage;
    public int? SuccessPercentage
    {
        get => _successPercentage;
        private set { _successPercentage = value; PercentageChanged?.Invoke(); }
    }

    public event System.Action? PercentageChanged;

    public QuestPreparationViewModel(QuestCard? quest = null)
    {
        SelectedQuest = quest;
    }

    public void AssignAdventurer(int slotIndex, AdventurerCard adventurer)
    {
        SelectedSlots[slotIndex] = adventurer;
        RecalculateSuccessRate();
    }

    private void RecalculateSuccessRate()
    {
        var chosen = SelectedSlots.Where(a => a != null).ToList();
        if (chosen.Count == 0) { SuccessPercentage = null; return; }

        // TODO: appel réel à la fonction de ton camarade
        // SuccessPercentage = QuestLogic.CalculerPourcentageVictoire(chosen, SelectedQuest);
        SuccessPercentage = 50;
    }
}