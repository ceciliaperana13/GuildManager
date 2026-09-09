namespace GuildManager.Domain.Entities;

public class CharacterEquipment
{
    public int Id { get; set; }
    public int GuildMemberCharacterId { get; set; }
    public int ItemId { get; set; }
    public string Slot { get; set; } = null!;
}
