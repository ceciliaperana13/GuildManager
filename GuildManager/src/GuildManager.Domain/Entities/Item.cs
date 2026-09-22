namespace GuildManager.Domain.Entities;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slot { get; set; } = null!;
    public string Rarity { get; set; } = null!;
    public int BonusAttack { get; set; }
    public int BonusDefence { get; set; }
    public int BonusMagic { get; set; }
    public int BonusHp { get; set; }
}
