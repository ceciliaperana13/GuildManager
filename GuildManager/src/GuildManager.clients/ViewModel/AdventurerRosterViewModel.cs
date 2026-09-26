using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GuildManager.Client.Models;
using GuildManager.Client.Services;
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
        _ = LoadAdventurersAsync();
    }

    private async Task LoadAdventurersAsync()
    {
        Adventurers.Clear();

        if (AppSession.IsCoop)
        {
            // Mode coop : le roster réel vit côté serveur (AdventurerManager en
            // singleton dans l'API), pas dans l'instance locale de Game. On va
            // donc le chercher via l'API plutôt que de lire game.adventurerManager.
            var apiClient = new GuildApiClient();
            var dtos = await apiClient.GetAdventurersRosterAsync();

            foreach (var dto in dtos)
            {
                AdventurerStatus status;
                if (dto.IsDead)
                    status = AdventurerStatus.Mort;
                else if (dto.IsHurted)
                    status = AdventurerStatus.Blesse;
                else if (dto.IsInQuest)
                    status = AdventurerStatus.EnQuete;
                else
                    status = AdventurerStatus.Disponible;

                Adventurers.Add(new AdventurerCard
                {
                    Name = dto.Name,
                    Level = dto.Lvl,
                    ClassName = dto.Job,
                    Health = dto.Health,
                    PhysicAttack = dto.PhysicAttack,
                    MagicAttack = dto.MagicAttack,
                    Defense = dto.Def,
                    Status = status,
                    PortraitPath = dto.Image
                });
            }
        }
        else
        {
            // Mode solo : la liste vit en mémoire dans Game.adventurerManager,
            // déjà à jour (recrutement, quêtes, etc.). Pas besoin de la
            // recharger depuis quoi que ce soit.
            foreach (Adventurer adventurer in Game.adventurerManager.adventurers)
            {
                AdventurerStatus status;
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
                    PortraitPath = adventurer.image
                });
            }
        }
    }
}