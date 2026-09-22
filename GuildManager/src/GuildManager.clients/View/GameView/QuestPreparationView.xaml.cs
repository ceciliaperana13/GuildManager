using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.ViewModel;
using GuildManager.Client.Models;

namespace GuildManager.Client.View;

public partial class QuestPreparationView : UserControl
{
    private int _activeSlot = -1;
    public QuestPreparationView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private QuestPreparationViewModel? ViewModel => DataContext as QuestPreparationViewModel;

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is QuestPreparationViewModel oldViewModel)
            oldViewModel.PercentageChanged -= UpdatePercentageDisplay;

        if (e.NewValue is QuestPreparationViewModel newViewModel)
        {
            newViewModel.PercentageChanged += UpdatePercentageDisplay;
            UpdatePercentageDisplay();
        }
    }

    private void OnSlot0Click(object sender, RoutedEventArgs e) => OpenPicker(0);
    private void OnSlot1Click(object sender, RoutedEventArgs e) => OpenPicker(1);
    private void OnSlot2Click(object sender, RoutedEventArgs e) => OpenPicker(2);

    private void OnAdventurerPicked(AdventurerCard adventurer)
    {
        if (_activeSlot < 0 || ViewModel is null) return;

        ViewModel.AssignAdventurer(_activeSlot, adventurer);
        _activeSlot = -1;
        Picker.Visibility = Visibility.Collapsed;
    }

      private void OpenPicker(int slotIndex)
    {
        _activeSlot = slotIndex;
        // TODO: filtrer sur les aventuriers "disponibles" une fois ce concept ajouté côté Domain
        Picker.SetAdventurers(new List<AdventurerCard>
       {
            new AdventurerCard { Name = "Alice" },
            new AdventurerCard { Name = "Bob" },
            new AdventurerCard { Name = "Charlie" }
       });
        
       Picker.Visibility = Visibility.Visible;

    }

    

     private void OnPickerCancelled() => Picker.Visibility = Visibility.Collapsed;

    private void UpdatePercentageDisplay()
    {
        if (ViewModel?.SuccessPercentage is int pct)
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