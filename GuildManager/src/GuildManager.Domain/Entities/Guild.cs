namespace GuildManager.Domain.Entities;

public class Guild
{
    public int Id { get; set; }
    public int FounderUserId { get; set; }
    public string Name { get; set; } = null!;
    public int Level { get; set; }
    public int Gold { get; set; }
    public int Food { get; set; }
    public int ReputationTotal { get; set; }
    public bool IsDefeated { get; set; }
    public DateTime CreatedAt { get; set; }
}
