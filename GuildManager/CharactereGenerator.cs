using System;
using System.Security.Cryptography;
public class CharacterGenerator
{
    public Adventurer generateCharacter(int lvl)
    {
        // choix de la classe aléatoire :
        string[] jobs = ["mage", "guerrier", "tank"];
        Random random = new Random();
        int index = random.Next(jobs.Length);
        string job = jobs[index];

        // génération des stats :
        int stats = 4 * lvl * 5; // à modifier pour l'équilibrage
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

        return new Adventurer(generateRandomName(type), job, lvl, health, defense, magic, physic, image, [], false);
    }

    public string generateRandomName(int type)
    {
        string[][] names = [["Aldric", "Théodran", "Kaelorn", "Eldran", "Gareth", "Valerian", "Draven", "Arthus", "Tharion", "Eryndor"], ["Elyria", "Isolde", "Aelwen", "Morgane", "Lysandra", "Elowen", "Seraphine", "Maëlys", "Nymeria", "Ariandel"]];

        Random random = new Random();
        return names[type-1][random.Next(names[type-1].Length)];
    }
}