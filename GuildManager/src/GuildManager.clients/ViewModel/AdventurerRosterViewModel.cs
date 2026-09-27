using System.Collections.ObjectModel;
using System.Linq;
using GuildManager.Client.Models;
using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.ViewModel;

public class AdventurerRosterViewModel : IScreenViewModel
{
    public string BackgroundPath => "/Assets/UI/table_avanturier.png";

    public ObservableCollection<AdventurerCard> Adventurers { get; } = new();

    public Game Game { get; }

    public AdventurerRosterViewModel(Game game)
    {
        Game = game;
        LoadAdventurers();
    }

    private void LoadAdventurers()
    {
        Adventurers.Clear();

        Game.adventurerManager.refreshAdventurers();

        var mainAdventurerIds = Game.adventurerManager.mainAdventurers
            .Select(m => m.id)
            .ToHashSet();

        foreach (Adventurer adventurer in Game.adventurerManager.mainAdventurers.Concat(Game.adventurerManager.adventurers.ToList()))
        {
            string framePath = mainAdventurerIds.Contains(adventurer.id)
                ? "/Assets/UI/cadre2.png"
                : "/Assets/UI/cadre.png";

            AdventurerStatus status;
            if (adventurer.isDead)
                status = AdventurerStatus.Mort;
            else if (adventurer.isHurted)
                status = AdventurerStatus.Blesse;
            else if (adventurer.isInQuest)
                status = AdventurerStatus.EnQuete;
            else
                status = AdventurerStatus.Disponible;

            Adventurers.Add(new AdventurerCard
            {
                Adventurer = adventurer,
                Name = adventurer.name,
                Level = adventurer.lvl,
                ClassName = adventurer.job,
                Health = adventurer.health,
                PhysicAttack = adventurer.physicAttack,
                MagicAttack = adventurer.magicAttack,
                Defense = adventurer.def,
                Status = status,
                PortraitPath = adventurer.image,
                FramePath = framePath
            });
        }
    }
}