using System.Collections.Generic;

namespace GuildManager.Client.Models;

public class Quest
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public List<string> RewardIconPaths { get; set; } = new();
    public bool IsGuildQuest { get; set; }
}