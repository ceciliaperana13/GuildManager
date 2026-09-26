using System.Collections.ObjectModel;
using GuildManager.Client.Models;
using GuildManager.Aplication.Guilds.Controls;
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
        foreach (Adventurer adventurer in game.adventurerManager.adventurers)
        {   
            if (adventurer.isHurted)
                status = AdventurerStatus.Blesse;
            else status = AdventurerStatus.Disponible;
            Adventurers.Add(new AdventurerCard { Adventurer = adventurer, Name = adventurer.name, Level = adventurer.lvl, ClassName = adventurer.job, Health = adventurer.health, PhysicAttack = adventurer.physicAttack, MagicAttack = adventurer.magicAttack, Defense = adventurer.def, Status = status, PortraitPath = adventurer.image });
        }
    }
}