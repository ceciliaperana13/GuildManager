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
        foreach (Adventurer mainAdventurer in game.adventurerManager.mainAdventurers)
        {   
            Adventurers.Add(new AdventurerCard { Adventurer = mainAdventurer, Name = mainAdventurer.name, Level = mainAdventurer.lvl, ClassName = mainAdventurer.job, Health = mainAdventurer.health, PhysicAttack = mainAdventurer.physicAttack, MagicAttack = mainAdventurer.magicAttack, Defense = mainAdventurer.def, Status = AdventurerStatus.Disponible, PortraitPath = mainAdventurer.image });
        }
        foreach (Adventurer adventurer in game.adventurerManager.adventurers)
        {   
            Adventurers.Add(new AdventurerCard { Adventurer = adventurer, Name = adventurer.name, Level = adventurer.lvl, ClassName = adventurer.job, Health = adventurer.health, PhysicAttack = adventurer.physicAttack, MagicAttack = adventurer.magicAttack, Defense = adventurer.def, Status = AdventurerStatus.Disponible, PortraitPath = adventurer.image });
        }
    }
}