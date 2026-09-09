namespace GuildManager.Domain.Entities;

public class Quest
{
    public int Id { get; set; }
    public int QuestTypeId { get; set; }
    public string Name { get; set; } = null!;
    public int RequiredGuildLevel { get; set; }
    public int StoryStep { get; set; }
    public int BaseSuccessRate { get; set; }
    public int MinPartySize { get; set; }
    public int MaxPartySize { get; set; }
}
