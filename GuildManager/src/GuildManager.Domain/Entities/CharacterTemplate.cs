namespace GuildManager.Domain.Entities;


public class CharacterClass
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
