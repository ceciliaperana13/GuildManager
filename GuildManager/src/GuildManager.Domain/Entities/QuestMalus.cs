namespace GuildManager.Domain.Entities;

//playerAt 
public class Turn
{
    public int Id { get; set; }
    public int GuildMemberId { get; set; }
    public int TurnNumber { get; set; }
    public int GuildGoldAfter { get; set; }
    public int GuildFoodAfter { get; set; }
    public DateTime PlayedAt { get; set; }
}
