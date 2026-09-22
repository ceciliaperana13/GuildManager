namespace GuildManager.Domain.Entities;

public class QuestAttempt
{
    public int Id { get; set; }
    public int QuestId { get; set; }
    public int GuildMemberId { get; set; }
    public int TurnNumber { get; set; }
    public int ComputedSuccessRate { get; set; }
    public bool IsSuccess { get; set; }
    public DateTime ResolvedAt { get; set; }
}
