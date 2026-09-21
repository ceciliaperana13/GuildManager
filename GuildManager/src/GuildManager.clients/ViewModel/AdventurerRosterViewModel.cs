using System.Collections.ObjectModel;
using GuildManager.Client.Models;
namespace GuildManager.Client.ViewModel;

public class AdventurerRosterViewModel : IScreenViewModel
{
    public string BackgroundPath => "/Assets/Backgrounds/table_avanturier.png";

    public ObservableCollection<Adventurer> Adventurers { get; } = new();

   public AdventurerRosterViewModel()
{
    // TODO: remplacer par une vraie requête EF Core sur GuildManagerDbContext
    // (ClassName sera fourni par la logique de classes d'un autre membre de l'équipe)

    Adventurers.Add(new Adventurer { Name = "???", Level = 1, Status = AdventurerStatus.Disponible, PortraitPath = "/Assets/UI/placeholder_portrait.png" });
    Adventurers.Add(new Adventurer { Name = "???", Level = 2, Status = AdventurerStatus.EnQuete, PortraitPath = "/Assets/UI/placeholder_portrait.png" });
    Adventurers.Add(new Adventurer { Name = "???", Level = 1, Status = AdventurerStatus.Blesse, PortraitPath = "/Assets/UI/placeholder_portrait.png" });
}
}