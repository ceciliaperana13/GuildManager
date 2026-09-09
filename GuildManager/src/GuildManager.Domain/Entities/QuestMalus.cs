namespace GuildManager.Domain.Entities;

public class QuestMalus
{
    public int Id { get; set; }
    public int QuestId { get; set; }
    public string MalusType { get; set; } = null!;
    public int Amount { get; set; }
}
