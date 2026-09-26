using System.Collections.ObjectModel;
using System.Linq;
using GuildManager.Aplication.Guilds.Controls;
using GuildManager.Client.Models;

namespace GuildManager.Client.ViewModel;

public class QuestPreparationViewModel : IScreenViewModel
{
    public string BackgroundPath => "/Assets/UI/Quest_background.png";
    public QuestCard? SelectedQuest { get; }
    public Game Game { get; }
    public IEnumerable<Monster> Enemies =>
    Game.questManager.searchQuestByName(SelectedQuest.Title).enemies;

    public ObservableCollection<AdventurerCard?> SelectedSlots { get; } = new() { null, null, null };

    private int? _successPercentage;
    public int? SuccessPercentage
    {
        get => _successPercentage;
        private set { _successPercentage = value; PercentageChanged?.Invoke(); }
    }

    public event System.Action? PercentageChanged;

    public QuestPreparationViewModel(Game game, QuestCard? quest = null)
    {
        Game = game;
        SelectedQuest = quest;
    }

    public IEnumerable<AdventurerCard> GetAvailableAdventurers()
    {
        if (Game.adventurerManager.adventurers.Count == 0)
            Game.adventurerManager.refreshAdventurers();

        var selectedIds = SelectedSlots
            .Where(adventurer => adventurer is not null)
            .Select(adventurer => adventurer!.Adventurer.id)
            .ToHashSet();

        return Game.adventurerManager.adventurers
            .Where(adventurer => !selectedIds.Contains(adventurer.id) && !adventurer.isHurted && !adventurer.isInQuest)
            .Select(adventurer => new AdventurerCard
            {
                Adventurer = adventurer,
                Name = adventurer.name,
                ClassName = adventurer.job,
                Health = adventurer.health,
                Defense = adventurer.def,
                MagicAttack = adventurer.magicAttack,
                PhysicAttack = adventurer.physicAttack,
                PortraitPath = adventurer.image,
                Level = adventurer.lvl,
                Status = AdventurerStatus.Disponible
            })
            .ToList();
    }

    public void AssignAdventurer(int slotIndex, AdventurerCard adventurer)
    {
        SelectedSlots[slotIndex] = adventurer;
        Game.questManager.searchQuestByName(SelectedQuest.Title).addAdventurer(adventurer.Adventurer);
        RecalculateSuccessRate();
    }

    private void RecalculateSuccessRate()
    {
        var chosen = SelectedSlots.Where(a => a != null).ToList();
        if (chosen.Count == 0) { SuccessPercentage = null; return; }

        
        // SuccessPercentage = QuestLogic.CalculerPourcentageVictoire(chosen, SelectedQuest);
        Quest quest = Game.questManager.searchQuestByName(SelectedQuest.Title);
        quest.refreshWinRate();
        SuccessPercentage = (int)quest.refreshWinRate();
    }

    public bool AcceptQuest()
    {
        return Game.questManager.searchQuestByName(SelectedQuest.Title).acceptQuest(Game.adventurerManager);
    }

    public bool GiveXp
{
    get => Game.questManager.searchQuestByName(SelectedQuest.Title).giveXp;
    set
    {
        Game.questManager.searchQuestByName(SelectedQuest.Title).giveXp = value;
    }
}
}