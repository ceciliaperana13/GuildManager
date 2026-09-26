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

    public static DialogueEntry? GetByTrigger(
    string trigger,
    int act,
    System.Func<string, bool>? conditionEvaluator = null,
    ISet<string>? excludedIds = null,
    bool? questSucceeded = null,
    string? completedStoryDialogueId = null)
{
    return _entries.Values.FirstOrDefault(entry =>
        entry.Trigger == trigger
        && entry.Act == act
        && (excludedIds is null || !excludedIds.Contains(entry.Id))
        && (trigger != "questResult" || !questSucceeded.HasValue || IsQuestResult(entry, questSucceeded.Value))
        && (trigger != "questResult" || completedStoryDialogueId is null || MatchesCompletedQuest(entry, completedStoryDialogueId))
        && HasRequiredDialogue(entry, excludedIds)
        && (entry.Condition is null || conditionEvaluator?.Invoke(entry.Condition) == true));
}

private static bool MatchesCompletedQuest(DialogueEntry entry, string completedStoryDialogueId)
{
    if (entry.RequiresDialogueIds is { Count: > 0 })
        return entry.RequiresDialogueIds.Contains(completedStoryDialogueId);

    return entry.RequiresDialogueId == completedStoryDialogueId;
}

    private static bool HasRequiredDialogue(DialogueEntry entry, ISet<string>? shownDialogueIds)
    {
        if (entry.RequiresDialogueIds is { Count: > 0 })
            return shownDialogueIds is not null
                && entry.RequiresDialogueIds.Any(shownDialogueIds.Contains);

        return entry.RequiresDialogueId is null
            || shownDialogueIds is not null && shownDialogueIds.Contains(entry.RequiresDialogueId);
    }

    private static bool IsQuestResult(DialogueEntry entry, bool succeeded)
    {
        var result = succeeded ? "victory" : "defeat";
        return entry.Id.Contains($"_{result}", System.StringComparison.OrdinalIgnoreCase);
    }
}