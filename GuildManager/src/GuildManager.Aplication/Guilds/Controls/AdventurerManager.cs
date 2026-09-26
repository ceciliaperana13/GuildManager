using System;
using System.Collections.Generic;
using System.Linq;

namespace GuildManager.Aplication.Guilds.Controls;

// AdventurerManager est désormais un conteneur PUREMENT EN MÉMOIRE.
// Il ne lit et n'écrit plus aucun fichier lui-même.
//
// La persistance est gérée ailleurs, à deux endroits différents selon le mode :
//   - Solo : GameSaveDto.FromGame(game) / .ToGame(), écrit par SaveSoloService
//            dans data/savesolo.json (une liste de sauvegardes complètes).
//   - Coop : GuildSaveState, écrit par JsonSaveFileStore (data/saves.json) côté API.
//
// Avant ce refactor, AdventurerManager essayait AUSSI de lire/écrire un fichier
// JSON directement (DataFile), avec un format incompatible avec celui utilisé
// par SaveSoloService (un objet {"adventurers": [...]} contre une liste de saves
// complètes) — c'est ce qui causait le crash "input does not contain any JSON tokens".
public class AdventurerManager
{
    public List<Adventurer> mainAdventurers;
    public List<Adventurer> adventurers;
    public List<Adventurer> adventurersToHire;

    // Compteur d'id propre à la save courante (remplace l'ancien idCount du json global)
    public int IdCounter { get; set; }

    public AdventurerManager()
    {
        this.adventurers = new List<Adventurer>();
        this.adventurersToHire = new List<Adventurer>();
        this.mainAdventurers = new List<Adventurer>();
        this.IdCounter = 0;
    }

    public Adventurer generateCharacter(int prestige)
    {
        string[] jobs = ["mage", "guerrier", "tank"];
        Random random = new Random();
        int index = random.Next(jobs.Length);
        string job = jobs[index];

        int lvl = random.Next((prestige - 1) * 10 + 1, prestige * 10 + 1);
        int stats = 4 * lvl * 5;
        int health = 0;
        int magic = 0;
        int physic = 0;
        int defense = 0;
        int type = random.Next(1, 3);

        if (job == "mage")
        {
            magic = random.Next(stats / 3, stats / 2);
            stats -= magic;
            health = random.Next(lvl / 2, stats);
            stats -= health;
            defense = random.Next(0, stats);
            stats -= defense;
            physic = stats;
        }

        if (job == "guerrier")
        {
            physic = random.Next(stats / 3, stats / 2);
            stats -= physic;
            health = random.Next(lvl / 2, stats);
            stats -= health;
            defense = random.Next(0, stats);
            stats -= defense;
            magic = stats;
        }

        if (job == "tank")
        {
            defense = random.Next(stats / 3, stats / 2 + 1);
            stats -= defense;
            health = random.Next(lvl, stats + 1);
            stats -= health;
            physic = random.Next(0, stats + 1);
            stats -= physic;
            magic = stats;
        }

        string image = $"/Assets/character/adventurers/{job + type}.png";

        // id local à la save, purement en mémoire
        IdCounter++;
        int newId = IdCounter;

        // Prices :
        int goldPrice = 100 + 20 * lvl; // à modifier selon les stats du perso
        int foodPrice = 10 + 2 * lvl;

        return new Adventurer(newId, generateRandomName(type), job, lvl, 0, health, defense, magic, physic, image, [], false, 0, false, false, goldPrice, foodPrice);
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

    public void refreshadventurersToHire(int prestige)
    {
        this.adventurersToHire.Clear();
        Console.WriteLine("Personnage à acheter : ");
        for (int i = 0; i < 3; i++)
        {
            Adventurer adventurer = this.generateCharacter(prestige);
            this.adventurersToHire.Add(adventurer);
        }
    }

    // Ancien comportement : rechargeait `adventurers` depuis un fichier JSON à
    // CHAQUE tour, ce qui aurait écrasé la progression du joueur. La liste
    // `adventurers` est maintenant l'état de jeu vivant, restauré une fois au
    // chargement de la save (GameSaveDto.ToGame()) puis mis à jour uniquement
    // par le recrutement / les quêtes. Ne fait donc plus rien ici.
    public void refreshAdventurers()
    {
        // no-op volontaire — voir commentaire ci-dessus
    }

    // Idem pour mainAdventurers. Si tu veux un roster de départ prédéfini pour
    // une NOUVELLE partie (pas à chaque tour), ajoute-le explicitement dans le
    // constructeur de Game ou dans SaveSoloService.CreateNewSave, pas ici.
    public void refreshMainAdventurers()
    {
        // no-op volontaire — voir commentaire ci-dessus
    }

    public void refreshAdventurersStatus(int turn)
    {
        foreach (Adventurer adventurer in this.adventurers.ToList())
        {
            if (adventurer.isDead)
                removeAdventurer(adventurer);
            else if (adventurer.isHurted && turn >= adventurer.hurtTurn + 3)
            {
                Console.WriteLine($"{adventurer.name} soigné");
                adventurer.AdventurerHeal();
            }
            editAdventurerData(adventurer.id, new Dictionary<string, JsonNode?>
            {
                ["isHurted"] = adventurer.isHurted,
                ["hurtTurn"] = adventurer.hurtTurn
                //["isInQuest"] = adventurer.isInQuest
            });
        }
    }

    public void AddAdventurer(Adventurer adventurer)
    {
        this.adventurers.Add(adventurer);
    }

    public void removeAdventurer(Adventurer adventurer)
    {
        this.adventurers.RemoveAll(a => a.id == adventurer.id);
    }

    public Adventurer? searchAdventurerById(int id)
    {
        foreach (Adventurer adventurer in this.adventurers)
        {
            if (adventurer.id == id)
                return adventurer;
        }
        return null;
    }

    public int adventurersEat()
    {
        int totalFood = 0;
        foreach (Adventurer adventurer in this.adventurers)
            totalFood += adventurer.foodPrice;

        foreach (Adventurer adventurer in this.mainAdventurers)
            totalFood += adventurer.foodPrice;

        Console.WriteLine($"Consomation ce tour : {totalFood}");
        return totalFood;
    }

    public void AdventurersLevelUp()
    {
        List<Adventurer> adventurersList = this.adventurers.Concat(this.mainAdventurers).ToList();
        foreach (Adventurer adventurer in adventurersList)
        {
            adventurer.levelUp();
            // levelUp() mute directement l'objet en mémoire ; plus de sync disque ici.
        }
    }

    public void SaveAdventurersAfterQuest(IEnumerable<Adventurer> participants)
    {
        // Les Adventurer sont des références : leurs stats (xp, lvl, health, ...)
        // ont déjà été mises à jour en mémoire par QuestManager pendant la quête.
        // Cette méthode ne fait donc plus rien : elle est gardée pour compatibilité
        // avec les appelants existants, au cas où une logique de validation /
        // notification serait ajoutée ici plus tard.
        // La persistance sur disque de l'état complet est déclenchée par
        // SaveSoloService.UpdateSave(saveId, game) (solo) ou par l'API coop.
    }
}