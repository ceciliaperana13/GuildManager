using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using GuildManager.Client.Models;

namespace GuildManager.Client.Services;

public static class DialogueRepository
{
    private static Dictionary<string, DialogueEntry> _entries = new();

    public static void Load()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "data", "Dialogue.json");
        var json = File.ReadAllText(path);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var list = JsonSerializer.Deserialize<List<DialogueEntry>>(json, options) ?? new();
        _entries = list.ToDictionary(e => e.Id);
    }

    public static DialogueEntry Get(string id) => _entries[id];

    public static DialogueEntry? GetByTrigger(string trigger, int act, System.Func<string, bool>? conditionEvaluator = null)
    {
        // TODO: logique de sélection réelle (act courant, condition à évaluer)
        return _entries.Values.FirstOrDefault(e => e.Trigger == trigger && e.Act == act);
    }
}