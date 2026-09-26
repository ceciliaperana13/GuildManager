using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace GuildManager.Aplication.Guilds.Controls;

public class AdventurerManager
{
    public List<Adventurer> mainAdventurers;
    public List<Adventurer> adventurers;
    public List<Adventurer> adventurersToHire;

    // Compteur d'id propre à la save courante (remplace idCount du json global)
    public int IdCounter { get; set; }

    public AdventurerManager()
    {
        this.adventurers = new List<Adventurer>();
        this.adventurersToHire = new List<Adventurer>();
        this.mainAdventurers = new List<Adventurer>();
        this.IdCounter = 0;
    }

    public Adventurer generateCharacter(int prestige)
    {
        string[] jobs = ["mage", "guerrier", "tank"];
        Random random = new Random();
        int index = random.Next(jobs.Length);
        string job = jobs[index];

        int lvl = random.Next((prestige - 1) * 10 + 1, prestige * 10 + 1);
        int stats = 4 * lvl * 5;
        int health = 0;
        int magic = 0;
        int physic = 0;
        int defense = 0;
        int type = random.Next(1, 3);

        if (job == "mage")
        {
            magic = random.Next(stats / 3, stats / 2);
            stats -= magic;
            health = random.Next(lvl / 2, stats);
            stats -= health;
            defense = random.Next(0, stats);
            stats -= defense;
            physic = stats;
        }

        if (job == "guerrier")
        {
            physic = random.Next(stats / 3, stats / 2);
            stats -= physic;
            health = random.Next(lvl / 2, stats);
            stats -= health;
            defense = random.Next(0, stats);
            stats -= defense;
            magic = stats;
        }

        if (job == "tank")
        {
            defense = random.Next(stats / 3, stats / 2 + 1);
            stats -= defense;
            health = random.Next(lvl, stats + 1);
            stats -= health;
            physic = random.Next(0, stats + 1);
            stats -= physic;
            magic = stats;
        }

        string image = $"/Assets/character/adventurers/{job + type}.png";

        // id local à la save, plus de lecture/écriture fichier ici
        IdCounter++;
        int newId = IdCounter;

        // Prices : 
        int goldPrice = 100 + 20*lvl; // à modifier selon les stats du perso
        int foodPrice = 10 + 2*lvl;

        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(DataFile, root.ToJsonString(options));

        return new Adventurer(idCount, generateRandomName(type), job, lvl, 0, health, defense, magic, physic, image, [], false, 0, false, false, goldPrice, foodPrice);
    }

    public string generateRandomName(int type)
    {
        string[][] names = [
            ["Aldric", "Théodran", "Kaelorn", "Eldran", "Gareth", "Valerian", "Draven", "Arthus", "Tharion", "Eryndor", "Roderic", "Alaric", "Kaelvar", "Darian", "Galdren", "Edrik", "Faelorn", "Lorcan", "Varendel", "Orvann"],
            ["Elyria", "Isolde", "Aelwen", "Morgane", "Lysandra", "Elowen", "Seraphine", "Maëlys", "Nymeria", "Ariandel", "Althéa", "Elaria", "Vaelith", "Rhianna", "Aveline", "Liora", "Thalyra", "Evania", "Miralys", "Faelina"]
        ];

        Random random = new Random();
        return names[type - 1][random.Next(names[type - 1].Length)];
    }

    public void addAdventurerToJson(Adventurer adventurer)
    {
        string json = File.ReadAllText(DataFile);
        JsonObject root = JsonNode.Parse(json)!.AsObject();


        var newAdventurer = new JsonObject
        {
            ["id"] = adventurer.id,
            ["name"] = adventurer.name,
            ["job"] = adventurer.job,
            ["lvl"] = adventurer.lvl,
            ["xp"] = adventurer.xp,
            ["health"] = adventurer.health,
            ["physicAttack"] = adventurer.physicAttack,
            ["magicAttack"] = adventurer.magicAttack,
            ["def"] = adventurer.def,
            ["image"] = adventurer.image,
            ["debuff"] = JsonSerializer.SerializeToNode(adventurer.debuff),
            ["isHurted"] = adventurer.isHurted,
            ["hurtTurn"] = adventurer.hurtTurn,
            ["isDead"] = adventurer.isDead,
            ["isInQuest"] = adventurer.isInQuest,
            ["goldPrice"] = adventurer.goldPrice,
            ["foodPrice"] = adventurer.foodPrice,

        };

        if (root["adventurers"] is not JsonArray adventurers)
        {
            adventurers = new JsonArray();
            root["adventurers"] = adventurers;
        }

        adventurers.Add(newAdventurer);

        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(DataFile, root.ToJsonString(options));
    }

    public List<Adventurer> generateAdventurerFromJson(string type)
    {
        string json = File.ReadAllText("data/adventurers.json");
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement adventurersJson = doc.RootElement.GetProperty(type);
        List<Adventurer> adventurers = JsonSerializer.Deserialize<List<Adventurer>>(adventurersJson.GetRawText()) ?? new List<Adventurer>();

        return adventurers;
    }

    public void refreshadventurersToHire(int prestige)
    {
        this.adventurersToHire.Clear();
        Console.WriteLine("Personnage à acheter : ");
        for (int i = 0; i < 3; i++)
        {
            Adventurer adventurer = this.generateCharacter(prestige);
            this.adventurersToHire.Add(adventurer);
            //adventurer.Write();
            //Console.WriteLine("\n");
        }
    }

    public void refreshAdventurers()
    {
        this.adventurers = generateAdventurerFromJson("adventurers");
    }

    public void refreshAdventurersStatus(int turn)
    {
        foreach (Adventurer adventurer in this.adventurers.ToList())
        {
            if (adventurer.isDead)
                removeAdventurer(adventurer);
            else if (adventurer.isHurted && turn >= adventurer.hurtTurn + 3)
            {
                Console.WriteLine($"{adventurer.name} soigné");
                adventurer.AdventurerHeal();
            }
            editAdventurerData(adventurer.id, new Dictionary<string, JsonNode?>
            {
                ["isHurted"] = adventurer.isHurted,
                ["hurtTurn"] = adventurer.hurtTurn,
                ["isInQuest"] = adventurer.isInQuest
            });
        }
    }

    public void refreshMainAdventurers()
    {
        this.mainAdventurers = generateAdventurerFromJson("mainAdventurers");
    }

    public void AddAdventurer(Adventurer adventurer)
    {
        this.adventurers.Add(adventurer);
    }

    public void removeAdventurer(Adventurer adventurer)
    {
        this.adventurers.RemoveAll(a => a.id == adventurer.id);
    }

    public Adventurer? searchAdventurerById(int id)
    {
        foreach (Adventurer adventurer in this.adventurers)
        {
            if (adventurer.id == id)
                return adventurer;
        }
        return null;
    }

    public int adventurersEat()
    {
        int totalFood = 0;
        foreach (Adventurer adventurer in this.adventurers)
            totalFood += adventurer.foodPrice;

        foreach (Adventurer adventurer in this.mainAdventurers)
            totalFood += adventurer.foodPrice;

        Console.WriteLine($"Consomation ce tour : {totalFood}");
        return totalFood;
    }

    public void AdventurersLevelUp()
    {
        List<Adventurer> adventurersList = this.adventurers.Concat(this.mainAdventurers).ToList();
        foreach(Adventurer adventurer in adventurersList)
        {
            adventurer.levelUp();
            editAdventurerData(adventurer.id, "lvl", adventurer.lvl);
        }
    }

    public void SaveAdventurersAfterQuest(IEnumerable<Adventurer> participants)
    {
        foreach (Adventurer adventurer in participants)
        {
            editAdventurerData(adventurer.id, new Dictionary<string, JsonNode?>
            {
                ["xp"] = adventurer.xp,
                ["lvl"] = adventurer.lvl,
                ["health"] = adventurer.health,
                ["physicAttack"] = adventurer.physicAttack,
                ["magicAttack"] = adventurer.magicAttack,
                ["def"] = adventurer.def,
                ["isHurted"] = adventurer.isHurted,
                ["hurtTurn"] = adventurer.hurtTurn,
                ["isDead"] = adventurer.isDead,
                ["isInQuest"] = adventurer.isInQuest
            });
        }
    }

    public void editAdventurerData(int id, string attribute, JsonNode? newValue)
    {
        editAdventurerData(id, new Dictionary<string, JsonNode?> { [attribute] = newValue });
    }

    public void editAdventurerData(int id, Dictionary<string, JsonNode?> updates)
    {
        string json = File.ReadAllText(DataFile);
        JsonObject root = JsonNode.Parse(json)!.AsObject();
        JsonArray adventurers = root["adventurers"]!.AsArray();

        JsonObject? target = adventurers
            .Select(a => a!.AsObject())
            .FirstOrDefault(a => a["id"]?.GetValue<int>() == id);

        if (target is null)
            throw new InvalidOperationException($"Aventurier '{id}' introuvable.");

        foreach (var kvp in updates)
            target[kvp.Key] = kvp.Value;

        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(DataFile, root.ToJsonString(options));
    }

    // public void SaveAdventurerStats(Adventurer adventurer)
    // {
    //     editAdventurerData(adventurer.id, new Dictionary<string, JsonNode?>
    //     {
    //         ["lvl"] = adventurer.lvl,
    //         ["xp"] = adventurer.xp,
    //         ["health"] = adventurer.health,
    //         ["physicAttack"] = adventurer.physicAttack,
    //         ["magicAttack"] = adventurer.magicAttack,
    //         ["def"] = adventurer.def
    //     });
    // }
}