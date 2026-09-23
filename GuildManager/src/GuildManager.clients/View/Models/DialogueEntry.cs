using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GuildManager.Client.Models;

public class DialogueEntry
{
    public string Id { get; set; } = "";
    public int Act { get; set; }
    public string Trigger { get; set; } = "";
    public string Speaker { get; set; } = "";
    public string image {get; set; } = "";
    public string? CharacterId { get; set; }
    public List<string> Lines { get; set; } = new();
    public List<DialogueChoice>? Choices { get; set; }
    public bool EndsGame { get; set; }
    public string? Condition { get; set; }
}

public class DialogueChoice
{
    public string Text { get; set; } = "";
    public Dictionary<string, object>? Effects { get; set; }
    public string? NextDialogueId { get; set; }
    public bool? EndsGame { get; set; }
}