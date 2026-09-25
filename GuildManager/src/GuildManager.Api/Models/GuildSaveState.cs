namespace GuildManager.Api.Models;


public class GuildSaveState
{
    // Guild ID. 1 = default shared co-op guild
    public int GuildId { get; set; } = 1;

    // Valeurs de départ pour une nouvelle guilde (utilisées uniquement
    // quand saves.json n'existe pas encore ou est absent/corrompu).
    public int Gold { get; set; } = 500;
    public int Food { get; set; } = 200;

    public int CurrentTurn { get; set; }

    // member
    public List<GuildMemberDto> Members { get; set; } = new();

    public List<CharacterSaveDto> Characters { get; set; } = new();

    //Flags 
    public List<string> StoryFlags { get; set; } = new();

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

public record GuildMemberDto(string Username, DateTime JoinedAt);

public class CharacterSaveDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string TemplateId { get; set; } = string.Empty;
    public int Level { get; set; }
    public bool IsCursed { get; set; }
    public bool IsPoisoned { get; set; }
}