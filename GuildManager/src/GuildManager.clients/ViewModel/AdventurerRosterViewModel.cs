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
        foreach (Adventurer adventurer in game.adventurerManager.adventurers)
        {   
            Adventurers.Add(new AdventurerCard { Name = adventurer.name, Level = adventurer.lvl, ClassName = adventurer.job, Health = adventurer.health, PhysicAttack = adventurer.physicAttack, MagicAttack = adventurer.magicAttack, Defense = adventurer.def, Status = AdventurerStatus.Disponible, PortraitPath = adventurer.image });
        }
    }
}