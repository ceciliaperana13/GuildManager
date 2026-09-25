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

    public string StatusText => IsLoading
        ? "Chargement des aventuriers..."
        : LoadError ?? (Candidates.Count == 0 ? "Aucun aventurier disponible." : "");

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            _isLoading = value;
            OnPropertyChanged(nameof(IsLoading));
            OnPropertyChanged(nameof(StatusText));
        }
    }

    private string? _loadError;
    public string? LoadError
    {
        get => _loadError;
        private set
        {
            _loadError = value;
            OnPropertyChanged(nameof(LoadError));
            OnPropertyChanged(nameof(StatusText));
        }
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
            foreach (var dto in dtos)
            {
                Candidates.Add(new AdventurerCandidate
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    PortraitPath = dto.Image,
                    RecruitmentCost = dto.GoldPrice
                });
            }

            OnPropertyChanged(nameof(StatusText));
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