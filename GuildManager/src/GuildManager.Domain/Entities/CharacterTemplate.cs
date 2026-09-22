namespace GuildManager.Domain.Entities;

public class CharacterTemplate
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int ClassId { get; set; }
    public bool CanSwitchToHealer { get; set; }
    public int BaseHp { get; set; }
    public int BaseAttack { get; set; }
    public int BaseDefence { get; set; }
    public int BaseMagic { get; set; }
    public int RecruitmentInLevel { get; set; }
    public int BaseRecruitmentCost { get; set; }
}