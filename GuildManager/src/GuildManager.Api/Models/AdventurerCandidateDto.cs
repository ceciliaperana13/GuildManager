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
    //attention je dois voir a pas changer la partie candidat car il sefface visuellement mais cela ne vas pas quand le turn ce passe remettre les candidat je suis obliger de partier de l'application .