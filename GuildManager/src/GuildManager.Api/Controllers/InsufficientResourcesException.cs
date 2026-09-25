namespace GuildManager.Api.Services;

public class InsufficientResourcesException : Exception
{
    public InsufficientResourcesException(string message) : base(message) { }
}