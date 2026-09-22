using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace GuildManager.Aplication.Guilds.Controls;
public class AdventurerManager
{
    private const string DataFile = "data/adventurers.json";
    public List<Adventurer> mainAdventurers;
    public List<Adventurer> adventurers;
    public List<Adventurer> adventurersToHire;

    public AdventurerManager()
    {
        this.adventurers = new List<Adventurer>();
        this.adventurersToHire = new List<Adventurer>();
        this.mainAdventurers = new List<Adventurer>();
    }

    public Adventurer generateCharacter(int prestige)
    {
        // choix de la classe aléatoire :
        string[] jobs = ["mage", "guerrier", "tank"];
        Random random = new Random();
        int index = random.Next(jobs.Length);
        string job = jobs[index];

        // génération des stats :
        int lvl = random.Next((prestige - 1) * 10 + 1, prestige * 10 + 1);
        int stats = 4 * lvl * 5; // à modifier selon l'équilibrage
        int health = 0;
        int magic = 0;
        int physic = 0;
        int defense = 0;
        int type = random.Next(1, 3);
        if (job == "mage")
        {
            magic = random.Next(stats/3, stats/2);
            stats -= magic;
            health = random.Next(lvl/2, stats);
            stats -= health;
            defense = random.Next(0, stats);
            stats -= defense;
            physic = stats;
        }

        if (job == "guerrier")
        {
            physic = random.Next(stats/3, stats/2);
            stats -= physic;
            health = random.Next(lvl/2, stats);
            stats -= health;
            defense = random.Next(0, stats);
            stats -= defense;
            magic = stats;
        }

        if (job == "tank")
        {
            defense = random.Next(stats/3, stats/2 + 1);
            stats -= defense;
            health = random.Next(lvl, stats + 1);
            stats -= health;
            physic = random.Next(0, stats + 1);
            stats -= physic;
            magic = stats;
        }
        string image = $"/Assets/character/adventurers/{job + type}.png";

        // def de l'id :
        string json = File.ReadAllText(DataFile);
        JsonObject root = JsonNode.Parse(json)!.AsObject();
        int idCount = root["idCount"]?.GetValue<int>() ?? 0;
        // incrementation de l'id
        root["idCount"] = idCount + 1;

        //def des prix
        int goldPrice = 100; // à modifier selon les stats du perso
        int foodPrice = 20;

        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(DataFile, root.ToJsonString(options));

        return new Adventurer(idCount, generateRandomName(type), job, lvl, health, defense, magic, physic, image, [], false, goldPrice, foodPrice);
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
            ["health"] = adventurer.health,
            ["physicAttack"] = adventurer.physicAttack,
            ["magicAttack"] = adventurer.magicAttack,
            ["def"] = adventurer.def,
            ["image"] = adventurer.image,
            ["debuff"] = JsonSerializer.SerializeToNode(adventurer.debuff),
            ["isHurted"] = adventurer.isHurted
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
        for(int i = 0; i < 3; i++)
        {
            Adventurer adventurer = this.generateCharacter(prestige);
            this.adventurersToHire.Add(adventurer);
            adventurer.Write();
            Console.WriteLine("\n");
        }
    }

    public void refreshAdventurers()
    {
        this.adventurers = generateAdventurerFromJson("adventurers");
    }

    public void refreshMainAdventurers()
    {
        this.mainAdventurers = generateAdventurerFromJson("mainAdventurers");
    }

    public void AddAdventurer(Adventurer adventurer)
    {
        this.adventurers.Add(adventurer);
        this.addAdventurerToJson(adventurer);        
    }

    public Adventurer searchAdventurerById(int id)
    {
        refreshAdventurers();
        foreach(Adventurer adventurer in this.adventurers)
        {
            if (adventurer.id == id)
                return adventurer;
        }
        //return new Adventurer(0, "", "", 0, 0, 0, 0, 0, "", [], false, 0, 0);
        return null;
    }

    public int adventurersEat()
    {
        int totalFood = 0;
        //random adventurers eat
        foreach(Adventurer adventurer in this.adventurers)
        {
            totalFood += adventurer.foodPrice;
        }
        //main adventurers eat
        foreach(Adventurer adventurer in this.mainAdventurers)
        {
            totalFood += adventurer.foodPrice;
        }
        return totalFood;
    }
}