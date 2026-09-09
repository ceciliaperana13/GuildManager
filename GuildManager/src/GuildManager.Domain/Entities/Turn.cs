namespace GuildManager.Domain.Entities;

public class GuildMemberCharacter
{
    public int Id { get; set; }
    public int GuildMemberId { get; set; }
    public int CharacterTemplateId { get; set; }
    public string Nickname { get; set; } = null!;
    public int Level { get; set; }
    public int Xp { get; set; }
    public int CurrentHp { get; set; }
    public string Status { get; set; } = null!;
    public bool IsHealerMode { get; set; }
    public DateTime RecruitedAt { get; set; }
}
