using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

public class CharacterGenerator
{
    private const string DataFile = "data/adventurers.json";

    public Adventurer generateCharacter(int lvl, int id)
    {
        // choix de la classe aléatoire :
        string[] jobs = ["mage", "guerrier", "tank"];
        Random random = new Random();
        int index = random.Next(jobs.Length);
        string job = jobs[index];

        // génération des stats :
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
        string image = $"assets/image/perso secondaires/{job + type}.png";

        return new Adventurer(id, generateRandomName(type), job, lvl, health, defense, magic, physic, image, [], false);
    }

    public string generateRandomName(int type)
    {
        string[][] names = [
            ["Aldric", "Théodran", "Kaelorn", "Eldran", "Gareth", "Valerian", "Draven", "Arthus", "Tharion", "Eryndor"],
            ["Elyria", "Isolde", "Aelwen", "Morgane", "Lysandra", "Elowen", "Seraphine", "Maëlys", "Nymeria", "Ariandel"]
        ];

        Random random = new Random();
        return names[type - 1][random.Next(names[type - 1].Length)];
    }

    public void addNewAdventurer(int lvl)
    {
        string json = File.ReadAllText(DataFile);
        JsonObject root = JsonNode.Parse(json)!.AsObject();

        // Source de vérité unique : "idCount" dans le fichier JSON
        int idCount = root["idCount"]?.GetValue<int>() ?? 0;

        Adventurer adventurer = generateCharacter(lvl, idCount);

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

        root["idCount"] = idCount + 1;

        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(DataFile, root.ToJsonString(options));
    }
}