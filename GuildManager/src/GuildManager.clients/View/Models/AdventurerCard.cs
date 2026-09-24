namespace GuildManager.Client.Models;

public enum AdventurerStatus
{
    Disponible,
    EnQuete,
    Blesse,
    Mort
}

public class AdventurerCard
{
    public string Name { get; set; } = "";
    public string ClassName { get; set; } = "";
    public string PortraitPath { get; set; } = "";
    public int Level { get; set; }
    public int Health { get; set; }
    public int PhysicAttack { get; set; }
    public int MagicAttack { get; set; }
    public int Defense { get; set; }
    public AdventurerStatus Status { get; set; }

    public string StatusLabel => Status switch
    {
        AdventurerStatus.Disponible => "Disponible",
        AdventurerStatus.EnQuete => "En quête",
        AdventurerStatus.Blesse => "Blessé",
        AdventurerStatus.Mort => "Mort",
        _ => ""
    };

    public string StatusColor => Status switch
    {
        AdventurerStatus.Disponible => "#4CAF50",
        AdventurerStatus.EnQuete => "#FFA726",
        AdventurerStatus.Blesse => "#EF5350",
        AdventurerStatus.Mort => "#616161",
        _ => "#FFFFFF"
    };
}