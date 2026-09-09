namespace GuildManager.Domain.Entities;


public class RecruitmentOffer
{
    public int Id { get; set; }
    public int GuildMemberId { get; set; }
    public int CharacterTemplateId { get; set; }
    public int TurnNumber { get; set; }
    public int GoldCost { get; set; }
    public bool IsPurchased { get; set; }
}
