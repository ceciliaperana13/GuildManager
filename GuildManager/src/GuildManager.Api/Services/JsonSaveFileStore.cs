using System.IO;
using System.Text.Json;
using GuildManager.Api.Models;

namespace GuildManager.Api.Services;

/// Store de sauvegarde basé sur un fichier JSON (data/savesolo.json ou data/saves.json).
/// Écriture atomique (fichier .tmp puis renommage) et verrou pour éviter les écritures
/// concurrentes (important surtout côté coop, où plusieurs requêtes API peuvent arriver
/// en même temps).

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
        if (!File.Exists(_path))
            return new GuildSaveState();

        await Lock.WaitAsync();
        try
        {
            await using var stream = File.OpenRead(_path);
            var state = await JsonSerializer.DeserializeAsync<GuildSaveState>(stream, Options);
            return state ?? new GuildSaveState();
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
            state.LastUpdated = DateTime.UtcNow;

            var tmpPath = _path + ".tmp";

            await using (var stream = File.Create(tmpPath))
                await JsonSerializer.SerializeAsync(stream, state, Options);

            File.Move(tmpPath, _path, overwrite: true);
        }
        finally
        {
            Lock.Release();
        }
    }
}