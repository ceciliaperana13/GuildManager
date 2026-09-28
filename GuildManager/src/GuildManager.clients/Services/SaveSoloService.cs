using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using GuildManager.Aplication.Guilds.Controls;

namespace GuildManager.Client.Services;

public class SaveSoloService
{
    private readonly string _filePath;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    public SaveSoloService(string? filePath = null)
    {
        // Same path logic as the coop save file (Path.Combine("data", "saves.json")
        // in CoopMenuView): relative to the current working directory, not the
        // build folder, so that both save files reside in the same location.
        _filePath = filePath ?? Path.Combine("data", "savesolo.json");
    }

    public List<GameSaveDto> LoadAll()
    {
        if (!File.Exists(_filePath))
            return new List<GameSaveDto>();

        var json = File.ReadAllText(_filePath);
        if (string.IsNullOrWhiteSpace(json))
            return new List<GameSaveDto>();

        try
        {
            return JsonSerializer.Deserialize<List<GameSaveDto>>(json, _jsonOptions)
                   ?? new List<GameSaveDto>();
        }
        catch (JsonException)
        {
            return new List<GameSaveDto>();
        }
    }

    private void SaveAll(List<GameSaveDto> saves)
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(saves, _jsonOptions);
        File.WriteAllText(_filePath, json);
    }

    /// Creates a new solo save and adds it to the existing list.
    public GameSaveDto CreateNewSave(Game game)
    {
        var saves = LoadAll();
        var newSave = GameSaveDto.FromGame(game);
        saves.Add(newSave);
        SaveAll(saves);
        return newSave;
    }

    /// Updates an existing save (e.g., after passTurn()).
    public void UpdateSave(Guid saveId, Game game)
    {
        var saves = LoadAll();
        var existing = saves.FirstOrDefault(s => s.SaveId == saveId);
        if (existing is null) return;

        var updated = GameSaveDto.FromGame(game);
        updated.SaveId = saveId;
        updated.CreatedAt = existing.CreatedAt;

        saves.Remove(existing);
        saves.Add(updated);
        SaveAll(saves);
    }

    public void DeleteSave(Guid saveId)
    {
        var saves = LoadAll();
        saves.RemoveAll(s => s.SaveId == saveId);
        SaveAll(saves);
    }

    /// Loads a specific save and reconstructs a playable Game from it.
    public Game? LoadGame(Guid saveId)
    {
        var saves = LoadAll();
        var save = saves.FirstOrDefault(s => s.SaveId == saveId);
        return save?.ToGame();
    }
}