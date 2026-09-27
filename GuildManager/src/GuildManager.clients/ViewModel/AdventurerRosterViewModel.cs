using System.Collections.ObjectModel;
using System.Linq;
using GuildManager.Client.Models;
using GuildManager.Aplication.Guilds.Controls;
using System.Windows.Controls;
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

    // Le roster est toujours local au joueur (solo comme coop) : seuls l'or et
    // la nourriture sont partagés entre joueurs, pas les aventuriers.
    private void LoadAdventurers()
    {
        Adventurers.Clear();

        foreach (Adventurer adventurer in Game.adventurerManager.adventurers)
        {
            AdventurerStatus status;
            if (adventurer.isDead)
                status = AdventurerStatus.Mort;
            else if (adventurer.isHurted)
        game.adventurerManager.refreshAdventurers();

        var mainAdventurerIds = game.adventurerManager.mainAdventurers
            .Select(m => m.id)
            .ToHashSet();

        AdventurerStatus status;
        string framePath = "";
        foreach (Adventurer adventurer in game.adventurerManager.mainAdventurers.Concat(game.adventurerManager.adventurers.ToList()))
        {
            framePath = mainAdventurerIds.Contains(adventurer.id)
                ? "/Assets/UI/cadre2.png"
                : "/Assets/UI/cadre.png";

            if (adventurer.isHurted)
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