namespace GuildManager.Domain.Entities;

public class QuestAttemptRewardLog
{
    public int Id { get; set; }
    public int QuestAttemptId { get; set; }
    public string RewardType { get; set; } = null!;
    public int Amount { get; set; }
    public int? ItemId { get; set; }
}
