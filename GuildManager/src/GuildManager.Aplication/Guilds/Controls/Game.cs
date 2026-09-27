using System;
using System.Collections.Generic;
using System.CodeDom;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using GuildManager.Client.Models;

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
    public bool IsPeriodicDialogueTurn { get; private set; }
    public int TurnsSinceActStart { get; private set; }
    public List<Item> inventory = new List<Item>();
    public AdventurerManager adventurerManager = new AdventurerManager();
    public QuestManager questManager = new QuestManager();
    public ItemManager itemManager = new ItemManager();
    bool isBought;
    bool turnInProgress;
    bool isCoop; // à utiliser pour le mode coop ?
    public string? LastCompletedStoryDialogueId { get; private set; }
    public bool IsDefeated() => this.gold <= 0;

    
    public List<QuestSummaryData> LastQuestSummaries { get; private set; } = new();

    // Coop = or/nourriture partagés via l'API ; aventuriers, candidats, xp, niveaux
    // et quêtes restent indépendants par joueur (mêmes règles qu'en solo).
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
         this.questManager.refreshQuests(this.prestige, this.storyFlags);
    }

    public void SetCoopMode(bool isCoop) => IsCoop = isCoop;

    // Achat en coop : l'or partagé est déjà débité côté API (voir
    // RecruitmentView.OnBuyClicked) avant l'appel à cette méthode.
    // L'aventurier reste local à ce joueur, comme en solo.
    public void AddPurchasedAdventurerCoop(Adventurer adventurer)
    {
        adventurerManager.AddAdventurer(adventurer);
        Console.WriteLine(adventurer.name + " recruté (coop).");
    }


    // Achat en coop : l'or partagé est déjà débité côté API (voir
    // RecruitmentView.OnBuyClicked) avant l'appel à cette méthode.
    // L'aventurier reste local à ce joueur, comme en solo.
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
        if (effect.Key == "unlockQuest" && effect.Value is JsonElement questJson
        && questJson.ValueKind == JsonValueKind.Object)
    {
        string questName = questJson.GetProperty("questName").GetString()!;
        int? expiresAfterTurns = questJson.TryGetProperty("expiresAfterTurns", out var turnsEl)
            ? turnsEl.GetInt32() : (int?)null;
        string? timeoutFlag = questJson.TryGetProperty("timeoutFlag", out var flagEl)
            ? flagEl.GetString() : null;
        string? introDialogueId = questJson.TryGetProperty("introDialogueId", out var introEl)
            ? introEl.GetString() : null;

        questManager.UnlockStoryQuest(questName, expiresAfterTurns, timeoutFlag, introDialogueId);
        continue;
    }

        if (effect.Value is JsonElement jsonValue && jsonValue.ValueKind == JsonValueKind.Number
            && jsonValue.TryGetInt32(out int amount))

            {
                storyFlags[effect.Key] = true;

                if (effect.Key.StartsWith("recruit_", StringComparison.Ordinal))
                    adventurerManager.RecruitMainAdventurer(effect.Key["recruit_".Length..]);
            }
        }
    }

    public bool HasStoryFlag(string condition)
    {
        var flag = condition.Replace("== true", "", StringComparison.OrdinalIgnoreCase).Trim();
        return storyFlags.TryGetValue(flag, out bool value) && value;
    }

    public void MarkDialogueAsShown(string dialogueId, int? advancesActTo = null)
{
    ShownDialogueIds.Add(dialogueId);

    if (dialogueId == "intro_01")
        AdvanceMainProgress(1);

    if (dialogueId == "act1_intro")
        adventurerManager.RecruitMainAdventurer("aventurier_prometteur");

    bool espritsOutcome = dialogueId is "act2_esprits_victory" or "act2_esprits_victory_cristal" or "act2_esprits_defeat";
    bool monstreOutcome = dialogueId is "act2_monstre_victory" or "act2_monstre_defeat";

    if (espritsOutcome)
    {
        storyFlags["act2_esprits_resolved"] = true;
        if (!ShownDialogueIds.Contains("act2_monstre_intro"))
            storyFlags["flag_act2_monstre"] = true;
    }

    if (monstreOutcome)
    {
        storyFlags["act2_monstre_resolved"] = true;
        if (!ShownDialogueIds.Contains("act2_esprits_intro"))
            storyFlags["flag_act2_esprits"] = true;
    }

    if (storyFlags.GetValueOrDefault("act2_esprits_resolved") && storyFlags.GetValueOrDefault("act2_monstre_resolved"))
        storyFlags["act2_both_resolved"] = true;

    if (advancesActTo.HasValue)
        AdvanceMainProgress(advancesActTo.Value);
}

    public void AdvanceMainProgress(int progress)
    {
        if (progress <= mainProgress) return;

        mainProgress = progress;
        TurnsSinceActStart = 0;
    }

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
    TurnsSinceActStart++;
    IsPeriodicDialogueTurn = TurnsSinceActStart % 5 == 0;
    this.isBought = false;
    LastQuestSucceeded = null;
    LastCompletedStoryDialogueId = null;

    var summaries = new List<QuestSummaryData>();

    foreach (Quest quest in this.questManager.quests)
    {
        if (quest.inProgress)
        {
            List<Adventurer> participants = new List<Adventurer>(quest.adventurers);

            Reward reward = this.questManager.completeQuestAndSave(quest, this.adventurerManager, this.turn);
            this.claimReward(reward);

            if (quest.StoryDialogueId is not null)
            {
                LastQuestSucceeded = quest.LastCompletionSucceeded;
                LastCompletedStoryDialogueId = quest.StoryDialogueId;
            }

            var adventurerCards = participants.Select(a => new AdventurerCard
            {
                Adventurer = a,
                Name = a.name,
                ClassName = a.job,
                Health = a.health,
                Defense = a.def,
                MagicAttack = a.magicAttack,
                PhysicAttack = a.physicAttack,
                PortraitPath = a.image,
                Level = a.lvl,
                Status = a.isDead ? AdventurerStatus.Mort
                    : a.isHurted ? AdventurerStatus.Blesse
                    : AdventurerStatus.Disponible
            }).ToList();

            int xpPerAdventurer = adventurerCards.Count > 0
                ? quest.LastXpShared / adventurerCards.Count
                : 0;

            summaries.Add(new QuestSummaryData(quest.name, quest.LastResultWon, adventurerCards, xpPerAdventurer, reward));
        }

        quest.markCompleted();
    }

    this.LastQuestSummaries = summaries;

    this.AdventurersRageQuit();

    if (this.questManager.refreshQuests(this.prestige, this.storyFlags))
        this.storyFlags["search_antagonist_timeout"] = true;

    this.refreshAll();

    this.food -= this.adventurerManager.adventurersEat();   
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