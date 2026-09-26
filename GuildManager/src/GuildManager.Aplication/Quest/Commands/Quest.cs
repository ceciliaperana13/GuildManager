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
    public Reward rewards { get; private set; }
    public int remainingTime { get; set; }
    public bool inProgress { get; private set; }
    public bool giveXp { get; set; }


    [JsonConstructor]
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
        this.giveXp = false;
        this.refreshWinRate();
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
            if (this.adventurers.Count == 0)
            {
                this.winRate = 0;
                return this.winRate;
            }

            double averageLevel = this.adventurers.Average(a => a.lvl);
            double delta = averageLevel - this.lvl;

            double bonusNum = 30.0 * Math.Log2(this.adventurers.Count + 1);
            double bonusEffectif = delta + bonusNum;

            this.winRate = 1.0 / (1.0 + Math.Pow(10, -bonusEffectif / 100.0)) * 100.0;
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
            if (teamPower + enemiesPower == 0)
                return 0; // évite une division par 0

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

    // AdventurerManager ne persiste plus rien sur disque (voir refactor
    // d'AdventurerManager) : la mutation en mémoire ci-dessous suffit,
    // la sauvegarde complète est écrite ailleurs (SaveSoloService / API coop).
    public bool acceptQuest(AdventurerManager adventurerManager)
    {
        if (this.adventurers.Count > 0)
        {
            this.inProgress = true;
            Console.WriteLine($"Quête acceptée : {this.name}");
            foreach (Adventurer adventurer in this.adventurers)
            {
                adventurer.isInQuest = true;
            }
            return true;
        }
        return false;
    }

    public bool completeQuest(int turn)
{
    this.inProgress = false;

    Random random = new Random();
    double num = random.NextDouble() * 100;

    if (num <= this.winRate) // victoire
    {
        foreach (Adventurer adventurer in this.adventurers)
        {
            adventurer.isInQuest = false;
            if (this.winRate <= 60 && this.type == "Combat" && random.Next(100) > 50)
                adventurer.adventurerHurt(turn);
        }
        Console.WriteLine($"Quête : {this.name} réussie");
        return true; 
    }
    else // défaite
    {
        foreach (Adventurer adventurer in this.adventurers)
        {
            adventurer.isInQuest = false;
            if (this.type == "Combat")
            {
                if (random.Next(100) < 25)
                    adventurer.adventurerHurt(turn);
                else if (random.Next(100) < 75)
                {
                    adventurer.isDead = true;
                    Console.Write($"{adventurer.name} est mort au combat {adventurer.isDead}");
                }
                else Console.WriteLine($"{adventurer.name} a fuit");    
            }
        }
        Console.WriteLine($"Quête : {this.name} échouée");
        return false; 
    }
}

    public Reward giveReward(int turn)
    {
        if (this.completeQuest(turn))
        {
            int adventurersXp = this.rewards.prestige / 2;
            if (this.giveXp)
            {
                Console.WriteLine("Tout l'xp à été atribué aux aventuriers");
                adventurersXp = this.rewards.prestige;
                this.rewards.prestige = 0;
            }
            else
                this.rewards.prestige /= 2;

            shareXp(adventurersXp);
            return this.rewards;
        }
        else
            return new Reward(0, 0, 0, []);
    }

    public void shareXp(int xp)
    {
        int personalXp = xp / this.adventurers.Count();
        foreach (Adventurer adventurer in this.adventurers)
        {
            adventurer.xp += personalXp;
            Console.WriteLine($"{adventurer.name} à reçut {personalXp} xp");
            adventurer.levelUp();
        }
    }
}