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

        var game = viewModel.Game;

        var adventurer = game.adventurerManager.adventurersToHire
            .FirstOrDefault(a => a.id == _selectedCandidate.Id);

        if (adventurer is null)
        {
            PurchaseText.Text = "Cet aventurier n'est plus disponible.";
            return;
        }

        if (AppSession.IsCoop)
        {
            if (game.gold < adventurer.goldPrice)
            {
                PurchaseText.Text = "Pas assez d'or dans le pot commun.";
                return;
            }
            if (game.food < adventurer.foodPrice)
            {
                PurchaseText.Text = "Pas assez de nourriture dans le pot commun.";
                return;
            }

            // Dépense locale, puis envoi de tout l'écart au pot commun partagé.
            game.SpendResources(adventurer.goldPrice, adventurer.foodPrice);

            if (!await CoopSync.PushAsync(game))
            {
                // Échec réseau : on annule la dépense locale
                game.SpendResources(-adventurer.goldPrice, -adventurer.foodPrice);
                PurchaseText.Text = "Achat impossible : serveur injoignable.";
                return;
            }

            game.AddPurchasedAdventurerCoop(adventurer);
            game.adventurerManager.adventurersToHire.Remove(adventurer);
        }
        else
        {
            bool bought = game.buyAdventurer(adventurer);
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