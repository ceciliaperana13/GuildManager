namespace GuildManager.Client.ViewModel;

public class AdventurerCandidate
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string ClassName { get; set; } = "";
    public int Level { get; set; }
    public int Health { get; set; }
    public int Defense { get; set; }
    public int PhysicAttack { get; set; }
    public int MagicAttack { get; set; }
    public string PortraitPath { get; set; } = "";
    public int RecruitmentCost { get; set; }
}