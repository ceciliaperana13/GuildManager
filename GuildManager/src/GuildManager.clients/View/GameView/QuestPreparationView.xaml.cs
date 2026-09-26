using System.Windows;
using System.Windows.Controls;
using GuildManager.Client.ViewModel;
using GuildManager.Client.Models;
using GuildManager.Client.Services;
using GuildManager.Aplication.Guilds.Controls;

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

    private async void OnSlot0Click(object sender, RoutedEventArgs e) => await OpenPicker(0);
    private async void OnSlot1Click(object sender, RoutedEventArgs e) => await OpenPicker(1);
    private async void OnSlot2Click(object sender, RoutedEventArgs e) => await OpenPicker(2);

    private void OnAdventurerPicked(AdventurerCard adventurer)
    {
        if (_activeSlot < 0 || ViewModel is null) return;

        ViewModel.AssignAdventurer(_activeSlot, adventurer);
        _activeSlot = -1;
        Picker.Visibility = Visibility.Collapsed;
    }

    private async System.Threading.Tasks.Task OpenPicker(int slotIndex)
    {
        _activeSlot = slotIndex;
        if (ViewModel is null) return;

        // En coop, on recharge le roster depuis l'API avant d'afficher le picker,
        // pour être sûr de voir les aventuriers achetés récemment (par soi ou un
        // coéquipier), sans dépendre d'un événement temps réel.
        if (ViewModel.Game.IsCoop)
        {
            var apiClient = new GuildApiClient();
            await ViewModel.Game.SyncCoopRosterAsync(apiClient);
        }

        Picker.SetAdventurers(ViewModel.GetAvailableAdventurers());
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

    private void OnEquipment0Click(object sender, RoutedEventArgs e) { /* TODO */ }
    private void OnEquipment1Click(object sender, RoutedEventArgs e) { /* TODO */ }
    private void OnEquipment2Click(object sender, RoutedEventArgs e) { /* TODO */ }

    private void OnLaunchQuestClicked(object sender, RoutedEventArgs e)
    {
        if (ViewModel.AcceptQuest())
            NavigationService.GoBack();
        else
        {
            OpenTextPopup("Choisissez au moins un aventurier avnat de lancer la quête.");
        }
    }

    private void OpenTextPopup(string message)
    {
        textPopup.SetText(message);
        textPopup.Visibility = Visibility.Visible;
    }

    private void OnTextPopupCancelled() => textPopup.Visibility = Visibility.Collapsed;
}