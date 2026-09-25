namespace GuildManager.Client.Services;

public static class AppSession
{
    public static int UserId { get; set; }
    public static string Username { get; set; } = string.Empty;
    public static string ApiBaseUrl { get; set; } = string.Empty;
    public static bool IsHost { get; set; }
    public static bool IsCoop { get; set; }
}