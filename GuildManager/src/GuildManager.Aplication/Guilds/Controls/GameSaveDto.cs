using System;
using System.Collections.Generic;

namespace GuildManager.Aplication.Guilds.Controls;

public class GameSaveDto
{
    public Guid SaveId { get; set; } = Guid.NewGuid();
    public string PlayerName { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastPlayedAt { get; set; } = DateTime.UtcNow;

    public int Turn { get; set; }
    public int MainProgress { get; set; }
    public int Gold { get; set; }
    public int Food { get; set; }
    public int Prestige { get; set; }
    public int Xp { get; set; }

    public Dictionary<string, bool> StoryFlags { get; set; } = new();
    public List<string> ShownDialogueIds { get; set; } = new();

    // Aventuriers propres à cette save
    public List<Adventurer> Adventurers { get; set; } = new();
    public List<Adventurer> MainAdventurers { get; set; } = new();
    public int AdventurerIdCounter { get; set; }

    public static GameSaveDto FromGame(Game game)
    {
        return new GameSaveDto
        {
            PlayerName = game.playerName,
            Turn = game.turn,
            MainProgress = game.mainProgress,
            Gold = game.gold,
            Food = game.food,
            Prestige = game.prestige,
            Xp = game.xp,
            StoryFlags = new Dictionary<string, bool>(game.storyFlags),
            ShownDialogueIds = new List<string>(game.ShownDialogueIds),
            Adventurers = new List<Adventurer>(game.adventurerManager.adventurers),
            MainAdventurers = new List<Adventurer>(game.adventurerManager.mainAdventurers),
            AdventurerIdCounter = game.adventurerManager.IdCounter,
            LastPlayedAt = DateTime.UtcNow
        };
    }

    // Reconstruit un Game à partir de cette save (pour le "Continuer")
    public Game ToGame()
    {
        var game = new Game(PlayerName, Turn, MainProgress, Gold, Food, Prestige, Xp);

        foreach (var flag in StoryFlags)
            game.storyFlags[flag.Key] = flag.Value;

        foreach (var id in ShownDialogueIds)
            game.MarkDialogueAsShown(id);

        game.adventurerManager.adventurers = new List<Adventurer>(Adventurers);
        game.adventurerManager.mainAdventurers = new List<Adventurer>(MainAdventurers);
        game.adventurerManager.IdCounter = AdventurerIdCounter;

        return game;
    }
}