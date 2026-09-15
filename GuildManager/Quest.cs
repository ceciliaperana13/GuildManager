using System;
using System.Collections.Generic;
using System.Linq;

public class Quest
{
    public string name { get; private set; }
    public string type { get; private set; }
    public int lvl { get; private set; }
    public string description { get; private set; }
    public List<Monster> enemies { get; private set; }
    public List<Adventurer> adventurers { get; private set; }
    public double winRate { get; private set; }
    public (int gold, int food, int prestige, List<Item> items) rewards { get; private set; } // faire une struct ?
    public int remainingTime { get; private set; }
    public bool inProgress { get; private set; }

    public Quest(string name, string type, int lvl, string description, List<Monster> enemies, List<Adventurer> adventurers, (int gold, int food, int prestige, List<Item>) rewards, int remainingTime, bool inProgress)
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

    double refreshWinRate()
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

        return teamPower / (teamPower + enemiesPower) * 100;
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
            return true; // cas de victoire
        else
            return false; // cas de défaite
    }
}