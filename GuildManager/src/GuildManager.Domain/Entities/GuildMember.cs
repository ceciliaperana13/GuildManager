namespace GuildManager.Domain.Entities;

public class GuildMember
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int GuildId { get; set; }
    public int Level { get; set; }
    public int Xp { get; set; }
    public int Reputation { get; set; }
    public bool IsFounder { get; set; }
    public DateTime JoinedAt { get; set; }
}
