using GuildManager.Aplication.Guilds.Controls;
using System.Collections.Generic;

namespace GuildManager.Client.Models;

public class QuestSummaryData
{
    public string QuestName { get; }
    public bool Victory { get; }
    public List<AdventurerCard> Adventurers { get; }
    public int XpPerAdventurer { get; }
    public Reward Reward { get; }

    public QuestSummaryData(string questName, bool victory, List<AdventurerCard> adventurers, int xpPerAdventurer, Reward reward)
    {
        QuestName = questName;
        Victory = victory;
        Adventurers = adventurers;
        XpPerAdventurer = xpPerAdventurer;
        Reward = reward;
    }
}