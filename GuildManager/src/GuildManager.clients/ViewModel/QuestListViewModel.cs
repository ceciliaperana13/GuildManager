using System.Collections.ObjectModel;
using GuildManager.Client.Models;
using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.ViewModel;

public class QuestListViewModel : IScreenViewModel
{
    public string BackgroundPath => "/Assets/UI/quest_board.png";

    public ObservableCollection<QuestCard> Quests { get; } = new();

    public QuestListViewModel(Game game)
    {
        LoadQuests(game);
    }

    private void LoadQuests(Game game)
    {
        game.questManager.refreshQuests(game.prestige);

        foreach (var logicQuest in game.questManager.quests)
        {
            Quests.Add(new QuestCard
            {
                Title = logicQuest.name,
                Description = logicQuest.description,
                IsGuildQuest = logicQuest.type == "Recherche"
            });
        }
    }
}