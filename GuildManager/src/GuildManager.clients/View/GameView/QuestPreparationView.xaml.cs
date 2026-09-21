using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.ViewModel;
using GuildManager.Client.Models;

namespace GuildManager.Client.View;

public partial class QuestPreparationView : UserControl
{
    private readonly QuestPreparationViewModel _viewModel;

    public QuestPreparationView()
    {
        InitializeComponent();
        _viewModel = new QuestPreparationViewModel();
        DataContext = _viewModel;
        _viewModel.PercentageChanged += UpdatePercentageDisplay;
    }

    private void UpdatePercentageDisplay()
    {
        if (_viewModel.SuccessPercentage is int pct)
        {
            PercentageText.Text = $"{pct}%";
            FillBar.Width = 460 * (pct / 100.0); // 460 = largeur intérieure approximative de la barre
        }
        else
        {
            PercentageText.Text = "--%";
            FillBar.Width = 0;
        }
    }

    private void OnSlot0Click(object sender, RoutedEventArgs e) => OpenPickerFor(0);
    private void OnSlot1Click(object sender, RoutedEventArgs e) => OpenPickerFor(1);
    private void OnSlot2Click(object sender, RoutedEventArgs e) => OpenPickerFor(2);

    private void OpenPickerFor(int slotIndex)
    {
        // TODO: afficher la popup de sélection filtrée sur les aventuriers Disponible
        // au choix de l'utilisateur : _viewModel.AssignAdventurer(slotIndex, adventurerChoisi);
    }

    private void OnEquipment0Click(object sender, RoutedEventArgs e) { /* TODO */ }
    private void OnEquipment1Click(object sender, RoutedEventArgs e) { /* TODO */ }
    private void OnEquipment2Click(object sender, RoutedEventArgs e) { /* TODO */ }

    private void OnLaunchQuestClicked(object sender, RoutedEventArgs e)
    {
        // TODO: lancer la quête avec les aventuriers/équipement choisis
    }
}