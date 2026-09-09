namespace GuildManager.Domain.Entities;

public class QuestAttemptMember
{
    public int Id { get; set; }
    public int QuestAttemptId { get; set; }
    public int GuildMemberCharacterId { get; set; }
    public bool Survived { get; set; }
    public int HpLost { get; set; }
}
