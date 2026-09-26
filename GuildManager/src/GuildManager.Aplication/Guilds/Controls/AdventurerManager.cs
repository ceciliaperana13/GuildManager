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

        // id :
        string json = File.ReadAllText(DataFile);
        JsonObject root = JsonNode.Parse(json)!.AsObject();
        int idCount = root["idCount"]?.GetValue<int>() ?? 0;
        root["idCount"] = idCount + 1;

        // Prices : 
        int goldPrice = 100 + 20*lvl; // à modifier selon les stats du perso
        int foodPrice = 20 + 5*lvl;

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
            ["isHurted"] = adventurer.isHurted,
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

    public Adventurer? RecruitMainAdventurer(string characterId)
    {
        refreshMainAdventurers();
        string name;
        string job;
        string image;
        int health;
        int defense;
        int magic;
        int physic;

        switch (characterId)
        {
            case "aventurier_prometteur":
                (name, job, image, health, defense, magic, physic) = ("Aventurier Prometteur", "guerrier", "/Assets/character/perso speciaux/epeiste.png", 50, 12, 4, 20);
                break;
            case "sorcier":
                (name, job, image, health, defense, magic, physic) = ("Sorcier", "mage", "/Assets/character/perso speciaux/sorcier2.png", 45, 6, 25, 8);
                break;
            case "alchimiste":
                (name, job, image, health, defense, magic, physic) = ("Alchimiste", "mage", "/Assets/character/perso speciaux/alchimiste.png", 40, 5, 22, 10);
                break;
            case "nain":
                (name, job, image, health, defense, magic, physic) = ("Nain", "tank", "/Assets/character/perso speciaux/tankNain1.png", 65, 25, 3, 10);
                break;
            default:
                return null;
        }

        Adventurer? existing = mainAdventurers.FirstOrDefault(adventurer => adventurer.image == image);
        if (existing is not null)
            return existing;

        string json = File.ReadAllText(DataFile);
        JsonObject root = JsonNode.Parse(json)!.AsObject();
        int id = root["idCount"]?.GetValue<int>() ?? 0;
        root["idCount"] = id + 1;

        Adventurer adventurer = new(id, name, job, 1, health, defense, magic, physic, image, [], false, 0, 10);
        mainAdventurers.Add(adventurer);

        if (root["mainAdventurers"] is not JsonArray mainAdventurersJson)
        {
            mainAdventurersJson = new JsonArray();
            root["mainAdventurers"] = mainAdventurersJson;
        }

        mainAdventurersJson.Add(JsonSerializer.SerializeToNode(adventurer));
        File.WriteAllText(DataFile, root.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        return adventurer;
    }

    public void removeAdventurer(Adventurer adventurer)
    {
        // remove on list
        this.adventurers.RemoveAll(a => a.id == adventurer.id);

        // remove on json
        string json = File.ReadAllText(DataFile);
        JsonObject root = JsonNode.Parse(json)!.AsObject();

        if (root["adventurers"] is JsonArray adventurersArray)
        {
            for (int i = adventurersArray.Count - 1; i >= 0; i--)
            {
                if (adventurersArray[i]?["id"]?.GetValue<int>() == adventurer.id)
                {
                    adventurersArray.RemoveAt(i);
                    break;
                }
            }
        }

        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(DataFile, root.ToJsonString(options));
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
        Console.WriteLine($"Consomation ce tour : {totalFood}");
        return totalFood;
    }
}