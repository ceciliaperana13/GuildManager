using System.Collections.ObjectModel;
using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.ViewModel;

public class RecruitmentViewModel : IScreenViewModel
{
    public string BackgroundPath => "/Assets/UI/table_selection.png";
    public Game Game { get; }

    public ObservableCollection<AdventurerCandidate> Candidates { get; } = new();

    public RecruitmentViewModel(Game game)
    {
        Game = game;
        // TODO: remplacer par l'appel réel à la fonction de ton coéquipier
        // Candidates = fonctionDeTirage.GetRandomCandidates(3);
        game.adventurerManager.refreshadventurersToHire(game.prestige);
        for(int i=0; i<3; i++)
        {
            var adventurer = game.adventurerManager.adventurersToHire[i];
            Candidates.Add(new AdventurerCandidate
            {
                Adventurer = adventurer,
                Name = adventurer.name,
                PortraitPath = adventurer.image,
                RecruitmentCost = adventurer.goldPrice
            });
        }
    }
}

public class AdventurerCandidate
{
    public Adventurer Adventurer { get; set; } = null!;
    public string Name { get; set; } = "";
    public string PortraitPath { get; set; } = "";
    public int RecruitmentCost { get; set; }
}