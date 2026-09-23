using System.Collections.ObjectModel;
using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.ViewModel;

public class RecruitmentViewModel : IScreenViewModel
{
    public string BackgroundPath => "/Assets/UI/table_selection.png";
    public Game Game { get; }
    public Game Game { get; }

    public ObservableCollection<AdventurerCandidate> Candidates { get; } = new();

    public RecruitmentViewModel(Game game)
    {
        Game = game;

        game.adventurerManager.refreshadventurersToHire(game.prestige);
        for(int i=0; i<3; i++)
        {
            var adventurer = game.adventurerManager.adventurersToHire[i];
            Candidates.Add(new AdventurerCandidate
            {
                Adventurer = adventurer,
                Name = adventurer.name,
                ClassName = adventurer.job,
                Level = adventurer.lvl,
                Health = adventurer.health,
                Defense = adventurer.def,
                MagicAttack = adventurer.magicAttack,
                PhysicAttack = adventurer.physicAttack,
                PortraitPath = adventurer.image,
                RecruitmentCost = adventurer.goldPrice
            });
        }
    }
}

public class AdventurerCandidate
{
    public Adventurer Adventurer { get; set; } = null!;
    public Adventurer Adventurer { get; set; } = null!;
    public string Name { get; set; } = "";
    public string ClassName { get; set; } = "";
    public int Level { get; set; }
    public int Health { get; set; }
    public int Defense { get; set; }
    public int PhysicAttack { get; set; }
    public int MagicAttack { get; set; }
    public string PortraitPath { get; set; } = "";
    public int RecruitmentCost { get; set; }
}