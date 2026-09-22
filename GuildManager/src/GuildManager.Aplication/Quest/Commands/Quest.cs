using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace GuildManager.Aplication.Guilds.Controls;

public class Quest
{
    public string name { get; private set; }
    public string type { get; private set; }
    public int lvl { get; private set; }
    public string description { get; private set; }
    public List<Monster> enemies { get; private set; }
    public List<Adventurer> adventurers { get; private set; }
    public double winRate { get; private set; }
    //public (int gold, int food, int prestige, List<Item> items) rewards { get; private set; } // faire une struct ?
    public Reward rewards { get; private set; }
    public int remainingTime { get; set; }
    public bool inProgress { get; private set; }

    [JsonConstructor]
    public Quest(string name, string type, int lvl, string description, List<Monster> enemies, List<Adventurer> adventurers, Reward rewards, int remainingTime, bool inProgress)
    {
        this.name = name;
        this.type = type;
        this.lvl = lvl;
        this.description = description;
        this.enemies = enemies;
        this.adventurers = adventurers;
        this.rewards = rewards;
        this.remainingTime = remainingTime;
        this.inProgress = inProgress;
        this.winRate = refreshWinRate();
    }

    public Quest(string name, string type, int lvl, string description, List<Monster> enemies, List<Adventurer> adventurers, Reward rewards, int remainingTime, bool inProgress, double winRate)
    {
        this.name = name;
        this.type = type;
        this.lvl = lvl;
        this.description = description;
        this.enemies = enemies;
        this.adventurers = adventurers;
        this.rewards = rewards;
        this.remainingTime = remainingTime;
        this.inProgress = inProgress;
        this.winRate = winRate;
    }

    void refreshCharacterPower()
    {
        List<Character> caracters = this.enemies.Cast<Character>()
            .Concat(this.adventurers.Cast<Character>())
            .ToList();

        foreach (Character character in caracters)
        {
            character.refreshPower();
        }
    }

    public double refreshWinRate()
    {
        if (this.type == "Recherche")
        {
            return this.winRate;
        }
        else
        {
            double enemiesPower = 0;
            double teamPower = 0;

            refreshCharacterPower();

            foreach (Monster enemy in this.enemies)
            {
                enemiesPower += enemy.power;
            }
            foreach (Adventurer adventurer in this.adventurers)
            {
                teamPower += adventurer.power;
                
            }
            Console.WriteLine("PUissance adventurers : " + teamPower);
            Console.WriteLine("PUissance enemy : " + enemiesPower);
            if (teamPower + enemiesPower == 0)
                return 0; // évite une division par 0

            //return teamPower / (teamPower + enemiesPower) * 100;
            //return 1/(1+10*(enemiesPower-teamPower)/100)*100;
            return 1.0 / (1.0 + Math.Pow(10, (enemiesPower - teamPower) / 100.0)) * 100.0;
        }
        
    }

    public void addAdventurer(Adventurer adventurer)
    {
        this.adventurers.Add(adventurer);
        this.winRate = refreshWinRate();
    }

    public void removeAdventurer(Adventurer adventurer)
    {
        this.adventurers.Remove(adventurer);
        this.winRate = refreshWinRate();
    }

    public void acceptQuest()
    {
        this.inProgress = true;
    }

    public bool completeQuest()
    {
        Random random = new Random();
        double num = random.NextDouble() * 100;

        if (num <= this.winRate)
        {
            Console.WriteLine($"Quête : {this.name} réussie");
            return true; // cas de victoire
        } 
        Console.WriteLine($"Quête : {this.name} échouée");
        return false; // cas de défaite
    }

    public Reward giveReward()
    {
        if (this.completeQuest())
        {
            //Console.WriteLine($"récompenses : {this.rewards.gold}");
            return this.rewards;
        }
        else 
            return new Reward(0, 0, 0, []);  
    }
}