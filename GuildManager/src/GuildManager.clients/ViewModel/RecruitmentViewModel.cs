using System.Collections.ObjectModel;
using System.ComponentModel;
using GuildManager.Aplication.Guilds.Controls;

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
        LoadCandidates();
    }

    // Candidats indépendants par joueur, en solo comme en coop : seuls l'or et la
    // nourriture sont partagés (cf. RecruitmentView.OnBuyClicked).
    private void LoadCandidates()
    {
        IsLoading = true;
        LoadError = null;

        try
        {
            Candidates.Clear();
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
        catch (System.Exception ex)
        {
            LoadError = $"Impossible de générer les candidats au recrutement : {ex.Message}";
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