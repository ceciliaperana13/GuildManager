
using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.ViewModel;
using GuildManager.Client.View.Controls;
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

    private void OnBuyClicked(object sender, RoutedEventArgs e)
    {
        if (_selectedCandidate is null || DataContext is not RecruitmentViewModel viewModel)
            return;

        viewModel.Game.buyAdventurer(_selectedCandidate.Adventurer);
        ClosePurchaseDialog();
    }

    private void OnCancelPurchaseClicked(object sender, RoutedEventArgs e) => ClosePurchaseDialog();

    private void ClosePurchaseDialog()
    {
        _selectedCandidate = null;
        PurchaseDialog.Visibility = Visibility.Collapsed;
    }

}