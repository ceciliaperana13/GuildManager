using System;
using System.IO;
using System.Text.Json;
using GuildManager.Api.Models;

namespace GuildManager.Api.Services;

public class JsonSaveFileStore : ISaveFileStore
{
    private readonly string _path;
    private static readonly SemaphoreSlim Lock = new(1, 1);
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public JsonSaveFileStore(string path)
    {
        _path = path;
        var directory = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);
    }

    public async Task<GuildSaveState> LoadAsync()
    {
        await Lock.WaitAsync();
        try
        {
            return await LoadInternalAsync();
        }
        finally
        {
            Lock.Release();
        }
    }

    public async Task SaveAsync(GuildSaveState state)
    {
        await Lock.WaitAsync();
        try
        {
            await SaveInternalAsync(state);
        }
        finally
        {
            Lock.Release();
        }
    }

    public async Task<GuildSaveState> UpdateAsync(Action<GuildSaveState> mutate)
    {
        await Lock.WaitAsync();
        try
        {
            var state = await LoadInternalAsync();
            mutate(state);
            await SaveInternalAsync(state);
            return state;
        }
        finally
        {
            Lock.Release();
        }
    }

    private async Task<GuildSaveState> LoadInternalAsync()
    {
        // Un fichier absent OU vide (0 octet) doit être traité pareil : on repart
        // d'un état de guilde neuf plutôt que de planter sur DeserializeAsync,
        // qui lève "input does not contain any JSON tokens" sur un flux vide.
        if (!File.Exists(_path) || new FileInfo(_path).Length == 0)
            return new GuildSaveState();

        try
        {
            await using var stream = File.OpenRead(_path);
            var state = await JsonSerializer.DeserializeAsync<GuildSaveState>(stream, Options);
            return state ?? new GuildSaveState();
        }
        catch (JsonException)
        {
            // Fichier présent mais corrompu/illisible : on ne bloque pas le jeu,
            // on repart d'un état neuf plutôt que de crasher au démarrage.
            return new GuildSaveState();
        }
    }

    private async Task SaveInternalAsync(GuildSaveState state)
    {
        state.LastUpdated = DateTime.UtcNow;
        var tmpPath = _path + ".tmp";

        await using (var stream = File.Create(tmpPath))
            await JsonSerializer.SerializeAsync(stream, state, Options);

        File.Move(tmpPath, _path, overwrite: true);
    }
}