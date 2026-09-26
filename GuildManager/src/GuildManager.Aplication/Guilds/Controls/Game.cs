using System;
using System.Collections.Generic;
using System.CodeDom;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using GuildManager.Client.Services;

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
    public Dictionary<string, bool> storyFlags { get; } = new();
    public HashSet<string> ShownDialogueIds { get; } = new();
    public bool? LastQuestSucceeded { get; private set; }
    public List<Item> inventory = new List<Item>();
    public AdventurerManager adventurerManager = new AdventurerManager();
    public QuestManager questManager = new QuestManager();
    public ItemManager itemManager = new ItemManager();
    bool isBought;
    bool turnInProgress;

    public bool IsCoop { get; private set; }

    public Game(string playerName, int turn, int mainProgress, int gold, int food, int prestige, int xp)
    {
        this.playerName = playerName;
        this.turn = turn;
        this.mainProgress = mainProgress;
        this.gold = gold;
        this.food = food;
        this.prestige = prestige;
        this.xp = xp;
        this.turnInProgress = true;
    }

    // À appeler une fois après le login, si AppSession.IsCoop est vrai (cf. LoginMenuView).
    // Charge le roster déjà recruté par la guilde coop et s'abonne aux futurs achats
    // faits par n'importe quel joueur (soi-même inclus) via SignalR.
    public async Task InitCoopAsync(GuildApiClient apiClient)
    {
        IsCoop = true;

        List<AdventurerCandidateDto> roster = await apiClient.GetAdventurersRosterAsync();
        foreach (AdventurerCandidateDto dto in roster)
        {
            if (adventurerManager.adventurers.Any(a => a.id == dto.Id))
                continue;
            adventurerManager.AddAdventurer(MapToAdventurer(dto));
        }

        // Évite les abonnements en double si InitCoopAsync est rappelé (ex: reconnexion).
        GuildRealtimeService.AdventurerHired -= OnAdventurerHired;
        GuildRealtimeService.AdventurerHired += OnAdventurerHired;
    }

    // Recharge le roster depuis l'API à la demande (ex: avant d'ouvrir le picker
    // de sélection de quête), sans dépendre de SignalR. Complète InitCoopAsync
    // pour garantir que les achats récents sont bien pris en compte localement.
    public async Task SyncCoopRosterAsync(GuildApiClient apiClient)
    {
        if (!IsCoop) return;

        List<AdventurerCandidateDto> roster = await apiClient.GetAdventurersRosterAsync();
        foreach (AdventurerCandidateDto dto in roster)
        {
            if (adventurerManager.adventurers.Any(a => a.id == dto.Id))
                continue;
            adventurerManager.AddAdventurer(MapToAdventurer(dto));
        }
    }

    private void OnAdventurerHired(AdventurerCandidateDto dto)
    {
        if (adventurerManager.adventurers.Any(a => a.id == dto.Id))
            return; // déjà présent

        adventurerManager.AddAdventurer(MapToAdventurer(dto));
    }

    private static Adventurer MapToAdventurer(AdventurerCandidateDto dto) => new Adventurer(
        dto.Id, dto.Name, dto.Job, dto.Lvl, dto.Xp, dto.Health, dto.Def,
        dto.MagicAttack, dto.PhysicAttack, dto.Image, dto.Debuff,
        dto.IsHurted, dto.HurtTurn, dto.IsDead, dto.IsInQuest,
        dto.GoldPrice, dto.FoodPrice);

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

    public bool buyItem(Item item)
    {
        if (this.gold >= item.goldPrice)
        {
            itemManager.addItem(item);
            this.gold -= item.goldPrice;
            Console.WriteLine(item.name + " acheté.");
            return true;
        }
        else
        {
            Console.WriteLine("Pas assez d'or pour acheter " + item.name);
            return false;
        }
        
    }

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

        this.xp += reward.prestige/2;

        this.prestigeUp();
    }

    public void ApplyDialogueEffects(Dictionary<string, object>? effects)
    {
        if (effects is null) return;

        foreach (var effect in effects)
        {
            if (effect.Value is JsonElement jsonValue && jsonValue.ValueKind == JsonValueKind.Number
                && jsonValue.TryGetInt32(out int amount))
            {
                switch (effect.Key)
                {
                    case "gold": gold += amount; break;
                    case "food": food += amount; break;
                    case "prestige": prestige += amount; break;
                    case "xp": xp += amount; break;
                }

                continue;
            }

            if (effect.Value is JsonElement jsonFlag && jsonFlag.ValueKind == JsonValueKind.True)
            {
                storyFlags[effect.Key] = true;

                if (effect.Key.StartsWith("recruit_", StringComparison.Ordinal))
                {
                    var adventurer = adventurerManager.generateCharacter(prestige);
                    adventurerManager.AddAdventurer(adventurer);
                }
            }
        }
    }

    public bool HasStoryFlag(string condition)
    {
        var flag = condition.Replace("== true", "", StringComparison.OrdinalIgnoreCase).Trim();
        return storyFlags.TryGetValue(flag, out bool value) && value;
    }

    public void MarkDialogueAsShown(string dialogueId) => ShownDialogueIds.Add(dialogueId);

    public bool Win()
    {
        if (this.prestige == 10)
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
        LastQuestSucceeded = null;
        foreach(Quest quest in this.questManager.quests)
        {
            if (quest.inProgress)
            {
                this.claimReward(this.questManager.completeQuestAndSave(quest, this.adventurerManager, this.turn));
            }
        }
        this.AdventurersRageQuit();
        this.refreshAll();
        this.food -= this.adventurerManager.adventurersEat();
        Console.WriteLine($"Bouffe : {this.food}");
    }

    public List<Adventurer> AdventurersRageQuit()
    {
        List<Adventurer> adventurersQuit = new List<Adventurer>();
        if (this.food <= 0)
        {
            Random random = new Random();
            foreach(Adventurer adventurer in this.adventurerManager.adventurers)
            {
                double value = random.NextDouble()*100;
                double chance = 50 - (30*(adventurer.lvl-1)/99);
                if (value < chance)
                    adventurersQuit.Add(adventurer);
                    
            }
        }
        foreach(Adventurer adventurer in adventurersQuit)
        {
            this.adventurerManager.removeAdventurer(adventurer);
            Console.WriteLine($"{adventurer.name} a quitter la guilde");
        }

        return adventurersQuit;
    }

    public void refreshAll()
    {
        this.questManager.refreshQuests(this.prestige);
        this.adventurerManager.refreshAdventurersStatus(this.turn);
        this.adventurerManager.refreshadventurersToHire(this.prestige);
        this.adventurerManager.refreshAdventurers();
        this.adventurerManager.refreshMainAdventurers();
        this.itemManager.refreshShop(this.prestige);
        this.itemManager.refreshInventory();
    }

    public void SyncResources(int gold, int food)
    {
        this.gold = gold;
        this.food = food;
    }
}