using System.Collections.ObjectModel;
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