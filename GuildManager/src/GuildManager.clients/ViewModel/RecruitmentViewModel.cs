using System.Collections.ObjectModel;
using System.ComponentModel;
using GuildManager.Aplication.Guilds.Controls;
using GuildManager.Client.Services;

namespace GuildManager.Client.ViewModel;

public class RecruitmentViewModel : IScreenViewModel, INotifyPropertyChanged
{
    public string BackgroundPath => "/Assets/UI/table_selection.png";
    public Game Game { get; }

    public ObservableCollection<AdventurerCandidate> Candidates { get; } = new();

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        private set { _isLoading = value; OnPropertyChanged(nameof(IsLoading)); }
    }

    private string? _loadError;
    public string? LoadError
    {
        get => _loadError;
        private set { _loadError = value; OnPropertyChanged(nameof(LoadError)); }
    }

    public RecruitmentViewModel(Game game)
    {
        Game = game;
        _ = LoadCandidatesAsync();
    }

    private async System.Threading.Tasks.Task LoadCandidatesAsync()
    {
        IsLoading = true;
        LoadError = null;

        try
        {
            Candidates.Clear();

            if (AppSession.IsCoop)
            {
                var apiClient = new GuildApiClient();
                var dtos = await apiClient.GetAdventurersToHireAsync();

                foreach (var dto in dtos)
                {
                    Candidates.Add(new AdventurerCandidate
                    {
                        Id = dto.Id,
                        Name = dto.Name,
                        ClassName = dto.Job,
                        Level = dto.Lvl,
                        Health = dto.Health,
                        Defense = dto.Def,
                        PhysicAttack = dto.PhysicAttack,
                        MagicAttack = dto.MagicAttack,
                        PortraitPath = dto.Image,
                        RecruitmentCost = dto.GoldPrice
                    });
                }
            }
            else
            {
                // Mode solo : génération locale des candidats via Game.adventurerManager,
                // aucun appel réseau nécessaire.
                Game.adventurerManager.refreshadventurersToHire(Game.prestige);

                foreach (var a in Game.adventurerManager.adventurersToHire)
                {
                    Candidates.Add(new AdventurerCandidate
                    {
                        Id = a.id,
                        Name = a.name,
                        ClassName = a.job,
                        Level = a.lvl,
                        Health = a.health,
                        Defense = a.def,
                        PhysicAttack = a.physicAttack,
                        MagicAttack = a.magicAttack,
                        PortraitPath = a.image,
                        RecruitmentCost = a.goldPrice
                    });
                }
            }
        }
        catch (System.Exception ex)
        {
            LoadError = $"Impossible de charger les candidats au recrutement : {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}