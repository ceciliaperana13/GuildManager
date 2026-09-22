using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.ViewModel;
using GuildManager.Client.Models;

namespace GuildManager.Client.View;

public partial class QuestPreparationView : UserControl
{
    private readonly QuestPreparationViewModel _viewModel;
    private int _activeSlot = -1;
    public QuestPreparationView()
    {
        InitializeComponent();
        _viewModel = new QuestPreparationViewModel();
        DataContext = _viewModel;
        _viewModel.PercentageChanged += UpdatePercentageDisplay;
    }

    private void OnSlot0Click(object sender, RoutedEventArgs e) => OpenPicker(0);
    private void OnSlot1Click(object sender, RoutedEventArgs e) => OpenPicker(1);
    private void OnSlot2Click(object sender, RoutedEventArgs e) => OpenPicker(2);

      private void OpenPicker(int slotIndex)
    {
        _activeSlot = slotIndex;
        // TODO: filtrer sur les aventuriers "disponibles" une fois ce concept ajouté côté Domain
        Picker.SetAdventurers(new List<Adventurer>
       {
            new Adventurer { Name = "Alice" },
            new Adventurer { Name = "Bob" },
            new Adventurer { Name = "Charlie" }
       });
        
       Picker.Visibility = Visibility.Visible;

    }

    

     private void OnPickerCancelled() => Picker.Visibility = Visibility.Collapsed;

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