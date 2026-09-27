namespace GuildManager.Api.Models;

public record AdventurerCandidateDto(
    int Id,
    string Name,
    string Job,
    int Lvl,
    int Xp,
    int Health,
    int Def,
    int MagicAttack,
    int PhysicAttack,
    string Image,
    List<string> Debuff,
    bool IsHurted,
    int HurtTurn,
    bool IsDead,
    bool IsInQuest,
    int GoldPrice,
    int FoodPrice);