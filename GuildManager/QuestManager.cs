using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

public class QuestManager
{
    public List<Quest> generateQuestsFromJson(string file)
    {
        string json = File.ReadAllText(file);
        List<Quest> quests = JsonSerializer.Deserialize<List<Quest>>(json) ?? new List<Quest>();

        return quests;
    }

    public Quest generateQuest(string type, int lvl, int levelGap = 5)
    {
        List<Quest> quests = generateQuestsFromJson("data/test.json"); // à modifier par le chemin du fichier
        List<Quest> availableQuests = new List<Quest>();

        foreach (Quest quest in quests)
        {
            if (quest.type == type
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
        string path = "data/test.json"; // à modifier
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
            ["isHurted"] = adventurer.isHurted
        };
        string path = "data/test.json";
        string json = File.ReadAllText(path);

        JsonArray quests = JsonNode.Parse(json)!.AsArray();

        JsonObject? target = quests
            .Select(q => q!.AsObject())
            .FirstOrDefault(q => q["name"]?.ToString() == questName);

        if (target is null)
            throw new InvalidOperationException($"Quête '{questName}' introuvable.");

        // si "adventurers" n'existe pas encore, on le crée
        if (target["adventurers"] is not JsonArray adventurers)
        {
            adventurers = new JsonArray();
            target["adventurers"] = adventurers;
        }

        adventurers.Add(newAdventurer);

        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(path, quests.ToJsonString(options));
    }

    
}