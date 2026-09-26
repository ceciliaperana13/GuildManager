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
        _filePath = filePath ?? Path.Combine(AppContext.BaseDirectory, "data", "savesolo.json");
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

    /// Crée une nouvelle save solo et l'ajoute à la liste existante.
    public GameSaveDto CreateNewSave(Game game)
    {
        var saves = LoadAll();
        var newSave = GameSaveDto.FromGame(game);
        saves.Add(newSave);
        SaveAll(saves);
        return newSave;
    }

    /// Met à jour une save existante (ex: après passTurn()).
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

    /// Charge une save précise et reconstruit un Game jouable à partir d'elle.
    public Game? LoadGame(Guid saveId)
    {
        var saves = LoadAll();
        var save = saves.FirstOrDefault(s => s.SaveId == saveId);
        return save?.ToGame();
    }
}