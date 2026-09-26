using System.Collections.ObjectModel;
using GuildManager.Client.Models;
using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.ViewModel;

public class QuestListViewModel : IScreenViewModel
{
    public string BackgroundPath => "/Assets/UI/quest_board.png";
    public Game Game { get; }

    public ObservableCollection<QuestCard> Quests { get; } = new();



    public QuestListViewModel(Game game)
{
    Game = game;
    LoadQuests(game);
}

private void LoadQuests(Game game)
{
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