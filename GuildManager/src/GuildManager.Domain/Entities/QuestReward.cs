namespace GuildManager.Domain.Entities;


public class QuestReward
{
    public int Id { get; set; }
    public int QuestId { get; set; }
    public string RewardType { get; set; } = null!;
    public int Amount { get; set; }
    public int? ItemId { get; set; }
}
