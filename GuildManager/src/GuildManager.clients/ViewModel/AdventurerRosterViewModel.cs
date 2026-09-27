using System.Collections.ObjectModel;
using GuildManager.Client.Models;
using GuildManager.Aplication.Guilds.Controls;
using System.Windows.Controls;
namespace GuildManager.Client.ViewModel;

public class AdventurerRosterViewModel : IScreenViewModel
{
    public string BackgroundPath => "/Assets/UI/table_avanturier.png";

    public ObservableCollection<AdventurerCard> Adventurers { get; } = new();

    public AdventurerRosterViewModel(Game game)
    {
        // TODO: remplacer par une vraie requête EF Core sur GuildManagerDbContext
        game.adventurerManager.refreshAdventurers();
        AdventurerStatus status;
        string framePath = "";
        foreach (Adventurer adventurer in game.adventurerManager.mainAdventurers.Concat(game.adventurerManager.adventurers.ToList()))
        {   
            if (adventurer.id <= 7)
                framePath = "/Assets/UI/cadre2.png";
            else 
                framePath = "/Assets/UI/cadre.png";
            if (adventurer.isHurted)
                status = AdventurerStatus.Blesse;
            else if (adventurer.isInQuest)
                status = AdventurerStatus.EnQuete;
            else status = AdventurerStatus.Disponible;
            Adventurers.Add(new AdventurerCard { Adventurer = adventurer, Name = adventurer.name, Level = adventurer.lvl, ClassName = adventurer.job, Health = adventurer.health, PhysicAttack = adventurer.physicAttack, MagicAttack = adventurer.magicAttack, Defense = adventurer.def, Status = status, PortraitPath = adventurer.image, FramePath = framePath});
        }
    }
}