namespace GuildManager.Api.Models;

public record AdventurerCandidateDto(
    int Id,
    string Name,
    string Job,
    int Lvl,
    int Health,
    int Def,
    int MagicAttack,
    int PhysicAttack,
    string Image,
    int GoldPrice,
    int FoodPrice);
    