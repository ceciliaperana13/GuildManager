using System.Collections.ObjectModel;
using GuildManager.Client.Models;

namespace GuildManager.Client.ViewModel;

public class QuestListViewModel : IScreenViewModel
{
    public string BackgroundPath => "/Assets/UI/quest_board.png";

    public ObservableCollection<Quest> Quests { get; } = new();

    public QuestListViewModel()
    {
        // TODO: charger les vraies quêtes disponibles (logique de jeu / API)
        Quests.Add(new Quest { Title = "???", Description = "???" });
        Quests.Add(new Quest { Title = "???", Description = "???" });
        Quests.Add(new Quest { Title = "???", Description = "???" });
    }
}