using System.CodeDom;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace GuildManager.Aplication.Guilds.Controls;
public class Game
{
    public string playerName {get; private set;}
    public int turn {get; set;}
    public int mainProgress {get; private set;}
    public int gold {get; private set;}
    public int food {get; private set;}
    public int prestige {get; private set;}
    public int xp {get; private set;}
    public List<Item> inventory = new List<Item>();
    public AdventurerManager adventurerManager = new AdventurerManager();
    public QuestManager questManager = new QuestManager();
    bool isBought;
    bool turnInProgress;
    bool isCoop; // à utiliser pour le mode coop ?


    public Game(string playerName, int turn, int mainProgress, int gold, int food, int prestige, int xp)
    {
        this.playerName = playerName;
        this.turn = turn;
        this.mainProgress = mainProgress;
        this.gold = gold;
        this.food = food;
        this.prestige = prestige;
        this.xp = xp;
        //this.adventurers = characterGenerator.generateAdventurerFromJson("adventurers");
        //this.adventurersToBuy = new List<Adventurer>();
        this.turnInProgress = true;
    }

    public void refreshSpecialadventurers()
    {
        
    }

    public bool buyAdventurer(Adventurer adventurer)
    {
        if (!this.isBought)
        {
            if (this.gold >= adventurer.goldPrice)
            {
                adventurerManager.AddAdventurer(adventurer);
                this.gold -= adventurer.goldPrice;
                Console.WriteLine(adventurer.name + " recruté.");
                this.isBought = true;
                return true;
            }
            else
            {
                Console.WriteLine("Pas assez d'or pour recruter " + adventurer.name);
                return false;
            }
        }
        else
        {
            Console.WriteLine("Vous avez déjà recruter un aventurier ce tour-ci");
            return false;
        }
        
    }

    // public void refreshQuest()
    // {
    //     foreach(Quest quest in this.questManager.quests)
    //     {
    //         if (quest.remainingTime <= 0 && !quest.inProgress)
    //         {
    //             this.questManager.quests.Remove(quest);
    //         }
    //         else
    //             quest.remainingTime--;
    //     }
    //     this.questManager.quests.Add(questManager.generateQuest("Combat", this.prestige, 100));// 100 à modifier ou enlevé
    //     this.questManager.quests.Add(questManager.generateQuest("Recherche", this.prestige, 100));// 100 à modifier ou enlevé
    // }  

    public void prestigeUp()
    {
        if (this.xp >= 100 * prestige)
        {
            this.prestige++;
            this.xp -= 100 * prestige;
        }
    }

    public void claimReward(Reward reward)
    {
        Console.WriteLine($"Vous avez gagné {reward.gold} gold, {reward.food} food et {reward.prestige} xp");
        this.gold += reward.gold;
        this.food += reward.food;
        this.xp += reward.prestige;
        // foreach (Item item in reward.Item4)
        // {
        //     this.inventory.Add(item);
        // }
        this.prestigeUp();
    }

    public bool Win()
    {
        if (this.prestige == 10) // ajouter la condition d'histoire terminée
        {
            Console.WriteLine("Partie Gagnée !");
            return true;
        } 
        return false;
    }

    public void passTurn()
    {
        this.turn++;
        this.isBought = false;
        // complétion des quêtes après le tour
        foreach(Quest quest in this.questManager.quests)
        {
            if (quest.inProgress)
            {
                this.claimReward(quest.giveReward());
            }
        }
        this.questManager.refreshQuests(this.prestige);
        this.adventurerManager.refreshadventurersToHire(this.prestige);
        this.adventurerManager.refreshAdventurers();
        this.adventurerManager.refreshMainAdventurers();
        this.food -= this.adventurerManager.adventurersEat();
        Console.WriteLine($"Bouffe : {this.food}");
    }

    // public void playTurn()
    // {   
    //     // complétion des quêtes après le tour
    //     foreach(Quest quest in this.questManager.quests)
    //     {
    //         if (quest.inProgress)
    //         {
    //             this.claimReward(quest.giveReward());
    //         }
    //     }


    //     this.turnInProgress = true;
    //     while (this.turnInProgress)
    //     {
    //         Console.WriteLine($"Tour {this.turn} | gold : {this.gold} | nouriture : {this.food} | prestige : {this.prestige}");
    //         Console.WriteLine("Recrutement : 1 | Quête : 2 | Tour suivant : 3");
    //         string action = Console.ReadLine();
    //         if (action == "1")
    //         {
    //             this.adventurerManager.refreshadventurersToHire(this.prestige);
    //             Console.WriteLine("Quel personnage voulez-vous acheter (1, 2 ou 3) : ");
    //             string choice = Console.ReadLine();
    //             this.buyAdventurer(this.adventurerManager.adventurersToHire[int.Parse(choice)-1]);
    //         }
    //         else if (action == "2")
    //         {
    //             this.questManager.refreshQuests(this.prestige);
    //             for (int i=0;i<=this.questManager.quests.Count-1;i++)
    //             {
    //                 Console.WriteLine($"Quête {i+1} : {this.questManager.quests[i].name}");
    //             }
    //             Console.WriteLine("Choisir la quête : ");
    //             string questChoice = Console.ReadLine();

    //             Console.WriteLine("Ennemies : ");
    //             foreach(Monster enemy in this.questManager.quests[int.Parse(questChoice)-1].enemies)
    //             {
    //                 enemy.Write();
    //             }

    //             Console.WriteLine("Aventuriers disponibles : ");
    //             this.adventurerManager.generateAdventurerFromJson("adventurers");
    //             foreach (Adventurer adventurer in this.adventurerManager.adventurers)
    //             {
    //                 adventurer.Write();
    //             }
    //             bool formingTeam = true;
    //             while (formingTeam)
    //             {
    //                 this.questManager.quests[int.Parse(questChoice)-1].refreshWinRate();
    //                 Console.WriteLine($"taux de réussite : {this.questManager.quests[int.Parse(questChoice)-1].winRate}\nChoisir l'aventurier 1 (ID) | A pour accepter la quête: ");
                        
    //                 string id = Console.ReadLine();
    //                 if (id == "a" && this.questManager.quests[int.Parse(questChoice)-1].adventurers.Count > 0 )
    //                 {
    //                     this.questManager.quests[int.Parse(questChoice)-1].acceptQuest();
    //                     formingTeam = false;
    //                 }
    //                 else
    //                 {
    //                     this.questManager.quests[int.Parse(questChoice)-1].addAdventurer(this.adventurerManager.searchAdventurerById(int.Parse(id)));
    //                 }
    //             }
    //         }      
    //         else {
    //             Console.WriteLine("Tour suivant");
    //             this.turnInProgress = false;
    //         }
    //     }
    // }
}