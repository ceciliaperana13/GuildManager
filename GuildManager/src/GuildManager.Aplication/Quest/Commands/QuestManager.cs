using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace GuildManager.Aplication.Guilds.Controls;
public class QuestManager
{
    public List<Quest> quests {get ; private set;}

    public QuestManager()
    {
        this.quests = new List<Quest>();
    }

   public List<Quest> generateQuestsFromJson(string file)
{
    string json = File.ReadAllText(file);

    List<Quest> quests = JsonSerializer.Deserialize<List<Quest>>(json) ?? new List<Quest>();

    return quests;
}

    public Quest generateQuest(string type, int lvl, int levelGap = 5)
    {
        List<Quest> quests = generateQuestsFromJson("data/quest.json"); 
        List<Quest> availableQuests = new List<Quest>();

        foreach (Quest quest in quests)
{
    if (!quest.IsStoryQuest
        && quest.type == type
        && quest.lvl >= lvl - levelGap
        && quest.lvl <= lvl + levelGap)
    {
        availableQuests.Add(quest);
    }
}

        if (availableQuests.Count == 0)
            throw new InvalidOperationException($"Aucune quête disponible pour le type '{type}' au niveau {lvl}.");

        Random random = new Random();
        int index = random.Next(availableQuests.Count);

        return availableQuests[index];
    }

    public void editQuestData(string questname, string attribute, JsonNode newValue)
    {
        string path = "data/quest.json";
        string json = File.ReadAllText(path);

        JsonArray quests = JsonNode.Parse(json)!.AsArray();

        JsonObject? target = quests
            .Select(q => q!.AsObject())
            .FirstOrDefault(q => q["name"]?.ToString() == questname);

        if (target is null)
            throw new InvalidOperationException($"Quête '{questname}' introuvable.");

        target[attribute] = newValue;

        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(path, quests.ToJsonString(options));
    }

    public void addAdventurerToQuest(string questName, Adventurer adventurer)
    {
        var newAdventurer = new JsonObject
        {
            ["id"] = adventurer.id,
            ["name"] = adventurer.name,
            ["job"] = adventurer.job,
            ["lvl"] = adventurer.lvl,
            ["health"] = adventurer.health,
            ["physicAttack"] = adventurer.physicAttack,
            ["magicAttack"] = adventurer.magicAttack,
            ["def"] = adventurer.def,
            ["image"] = adventurer.image,
            ["debuff"] = JsonSerializer.SerializeToNode(adventurer.debuff),
            ["isHurted"] = adventurer.isHurted,
            ["goldPrice"] = adventurer.goldPrice,
            ["foodPrice"] = adventurer.foodPrice
        };
        string path = "data/quest.json";
        string json = File.ReadAllText(path);

        JsonArray quests = JsonNode.Parse(json)!.AsArray();

        JsonObject? target = quests
            .Select(q => q!.AsObject())
            .FirstOrDefault(q => q["name"]?.ToString() == questName);

        if (target is null)
            throw new InvalidOperationException($"Quête '{questName}' introuvable.");

        // if "adventurers" does not yet exist, create it
        if (target["adventurers"] is not JsonArray adventurers)
        {
            adventurers = new JsonArray();
            target["adventurers"] = adventurers;
        }

        adventurers.Add(newAdventurer);

        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(path, quests.ToJsonString(options));
    }

    public bool refreshQuests(int prestige, IDictionary<string, bool>? storyFlags = null)
    {
        //remove old quests
        List<Quest> newQuests = new List<Quest>();
        bool researchQuestExpired = false;
        foreach(Quest quest in this.quests)
        {
            if (quest.type == "Recherche" && !quest.inProgress && quest.remainingTime <= 1)
                researchQuestExpired = true;

            quest.remainingTime--;
        if (quest.remainingTime > 0)
        {
            newQuests.Add(quest);
        }
        else if (quest.TimeoutFlag is not null && storyFlags is not null && quest.LastCompletionSucceeded is null)
        {
            // the quest was never accepted/resolved -> it actually expires
            storyFlags[quest.TimeoutFlag] = true;
        }   
        }
        this.quests = newQuests;

        // creation of new quests
        List<string> types = ["Combat", "Recherche"];
        if (prestige >= 4 && !types.Contains("Donjon"))
            types.Add("Donjon");

        Random random = new Random();
        foreach (string type in types)
        {
            this.quests.Add(generateQuest(type, random.Next((prestige-1)*10+1, prestige*10-1), 100));
        }

        return researchQuestExpired;
    }

    public Quest searchQuestByName(string name)
{
    foreach (Quest quest in this.quests)
        if (quest.name == name) return quest;
    return null;
}

public Quest UnlockStoryQuest(string questName, int? expiresAfterTurns, string? timeoutFlag, string? introDialogueId)
{
    Quest? existing = this.quests.FirstOrDefault(q => q.name == questName && q.IsStoryQuest);
    if (existing is not null)
        return existing; // already unlocked; do not duplicate

    Quest template = generateQuestsFromJson("data/quest.json")
        .FirstOrDefault(q => q.name == questName)
        ?? throw new InvalidOperationException($"Quête '{questName}' introuvable dans quest.json.");

    if (expiresAfterTurns.HasValue)
        template.remainingTime = expiresAfterTurns.Value;

    template.SetTimeoutFlag(timeoutFlag);
    template.SetStoryDialogueId(introDialogueId);

    this.quests.Add(template);
    return template;
}

    public Reward completeQuestAndSave(Quest quest, AdventurerManager adventurerManager, int turn)
    {
        List<Adventurer> participants = new List<Adventurer>(quest.adventurers);
        Reward reward = quest.giveReward(turn);
        adventurerManager.SaveAdventurersAfterQuest(participants);

        return reward;
    }
}