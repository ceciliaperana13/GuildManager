using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.ViewModel;
using GuildManager.Client.View.Controls;
using GuildManager.Client.Services;
using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.View;

public partial class RecruitmentView : UserControl
{
    private AdventurerCandidate? _selectedCandidate;
    public RecruitmentView()
    {
        InitializeComponent();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Scroll.PlayOpenAnimation();
    }

    private void OnCandidateClicked(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement element || element.DataContext is not AdventurerCandidate candidate)
            return;

        _selectedCandidate = candidate;
        PurchaseText.Text = $"Voulez-vous recruter {candidate.Name} pour {candidate.RecruitmentCost} pièces d'or ?";
        PurchaseDialog.Visibility = Visibility.Visible;
    }

    private async void OnBuyClicked(object sender, RoutedEventArgs e)
    {
        if (_selectedCandidate is null || DataContext is not RecruitmentViewModel viewModel)
            return;

        if (AppSession.IsCoop)
        {
            // Mode coop : l'achat passe par l'API, qui gère le pot commun partagé.
            var apiClient = new GuildApiClient();
            var (success, resources, error) = await apiClient.HireAdventurerAsync(_selectedCandidate.Id);

            if (!success)
            {
                PurchaseText.Text = error ?? "Achat impossible.";
                return; // laisse la boîte de dialogue ouverte pour montrer l'erreur
            }

            viewModel.Game.SyncResources(resources!.Gold, resources.Food);
        }
        else
        {
            // Mode solo : l'achat se fait directement en local sur l'instance de Game.
            var adventurer = viewModel.Game.adventurerManager.adventurersToHire
                .FirstOrDefault(a => a.id == _selectedCandidate.Id);

            if (adventurer is null)
            {
                PurchaseText.Text = "Cet aventurier n'est plus disponible.";
                return;
            }

            bool bought = viewModel.Game.buyAdventurer(adventurer);
            if (!bought)
            {
                PurchaseText.Text = "Pas assez d'or, ou recrutement déjà effectué ce tour-ci.";
                return;
            }
        }

        viewModel.Candidates.Remove(_selectedCandidate);
        ClosePurchaseDialog();
    }

    private void OnCancelPurchaseClicked(object sender, RoutedEventArgs e) => ClosePurchaseDialog();

    private void ClosePurchaseDialog()
    {
        _selectedCandidate = null;
        PurchaseDialog.Visibility = Visibility.Collapsed;
    }

}