using System.Collections.ObjectModel;
using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.ViewModel;

public class RecruitmentViewModel : IScreenViewModel
{
    public string BackgroundPath => "/Assets/UI/table_selection.png";

    public ObservableCollection<AdventurerCandidate> Candidates { get; } = new();

    public RecruitmentViewModel(Game game)
    {
        // TODO: remplacer par l'appel réel à la fonction de ton coéquipier
        // Candidates = fonctionDeTirage.GetRandomCandidates(3);
        game.adventurerManager.refreshadventurersToHire(game.prestige);
        for(int i=0; i<3; i++)
        {
            Candidates.Add(new AdventurerCandidate { Name = game.adventurerManager.adventurersToHire[i].name, PortraitPath = game.adventurerManager.adventurersToHire[i].image});
        }
    }
}

public class AdventurerCandidate
{
    public string Name { get; set; } = "";
    public string PortraitPath { get; set; } = "";
    public int RecruitmentCost { get; set; }
}