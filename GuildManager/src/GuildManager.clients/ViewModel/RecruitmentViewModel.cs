using System.Collections.ObjectModel;

namespace GuildManager.Client.ViewModel;

public class RecruitmentViewModel : IScreenViewModel
{
    public string BackgroundPath => "/Assets/UI/table_selection.png";

    public ObservableCollection<AdventurerCandidate> Candidates { get; } = new();

    public RecruitmentViewModel()
    {
        // TODO: remplacer par l'appel réel à la fonction de ton coéquipier
        // Candidates = fonctionDeTirage.GetRandomCandidates(3);

        // Placeholder temporaire en attendant :
        Candidates.Add(new AdventurerCandidate { Name = "???", PortraitPath = "/Assets/UI/placeholder_portrait.png" });
        Candidates.Add(new AdventurerCandidate { Name = "???", PortraitPath = "/Assets/UI/placeholder_portrait.png" });
        Candidates.Add(new AdventurerCandidate { Name = "???", PortraitPath = "/Assets/UI/placeholder_portrait.png" });
    }
}

public class AdventurerCandidate
{
    public string Name { get; set; } = "";
    public string PortraitPath { get; set; } = "";
    public int RecruitmentCost { get; set; }
}