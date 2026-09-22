namespace GuildManager.Domain.Entities;

public class GuildInventory
{
    public int Id { get; set; }
    public int GuildId { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
}
