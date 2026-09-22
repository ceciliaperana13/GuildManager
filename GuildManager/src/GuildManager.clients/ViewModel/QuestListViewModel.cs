using System;
using System.Collections.ObjectModel;
using System.IO;
using GuildManager.Client.Models;
using GameLogic = GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.ViewModel;

public class QuestListViewModel : IScreenViewModel
{
    public string BackgroundPath => "/Assets/UI/quest_board.png";

    public ObservableCollection<Quest> Quests { get; } = new();

    public QuestListViewModel()
    {
        LoadQuests();
    }

    private void LoadQuests()
    {
        var questsFilePath = Path.Combine(AppContext.BaseDirectory, "data", "quest.json");

        var questManager = new GameLogic.QuestManager();
        var logicQuests = questManager.generateQuestsFromJson(questsFilePath);

        foreach (var logicQuest in logicQuests)
        {
            Quests.Add(new Quest
            {
                Title = logicQuest.name,
                Description = logicQuest.description,
                IsGuildQuest = logicQuest.type == "Recherche"
            });
        }
    }
}