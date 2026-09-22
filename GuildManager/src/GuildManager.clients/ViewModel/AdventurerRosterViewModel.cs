using System.Collections.ObjectModel;
using GuildManager.Client.Models;
using GuildManager.Aplication.Guilds.Controls;
namespace GuildManager.Client.ViewModel;

public class AdventurerRosterViewModel : IScreenViewModel
{
    public string BackgroundPath => "/Assets/Backgrounds/table_avanturier.png";

    public ObservableCollection<AdventurerCard> Adventurers { get; } = new();

    public AdventurerRosterViewModel(Game game)
    {
        // TODO: remplacer par une vraie requête EF Core sur GuildManagerDbContext
        // (ClassName sera fourni par la logique de classes d'un autre membre de l'équipe)
        foreach (Adventurer adventurer in game.adventurerManager.adventurers)
        {   
            Adventurers.Add(new AdventurerCard { Name = adventurer.name, Level = adventurer.lvl, Status = AdventurerStatus.Disponible, PortraitPath = adventurer.image });
        }
        
        // Adventurers.Add(new AdventurerCard { Name = "???", Level = 2, Status = AdventurerStatus.EnQuete, PortraitPath = "/Assets/UI/placeholder_portrait.png" });
        // Adventurers.Add(new AdventurerCard { Name = "???", Level = 1, Status = AdventurerStatus.Blesse, PortraitPath = "/Assets/UI/placeholder_portrait.png" });
    }
}