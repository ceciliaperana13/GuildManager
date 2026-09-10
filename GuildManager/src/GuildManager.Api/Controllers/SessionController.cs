using GuildManager.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GuildManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionController : ControllerBase
{
    private readonly PlayerConnectionTracker _tracker;
    private readonly ILogger<SessionController> _logger;

    public SessionController(PlayerConnectionTracker tracker, ILogger<SessionController> logger)
    {
        _tracker = tracker;
        _logger = logger;
    }

    public record HelloRequest(string PlayerName);

    [HttpPost("hello")]
    public IActionResult Hello(HelloRequest request)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "inconnu";
        _tracker.Register(request.PlayerName, ip);
        _logger.LogInformation("✅ Joueur connecté : {PlayerName} depuis {Ip}", request.PlayerName, ip);
        return Ok(new { status = "connected" });
    }

    [HttpPost("heartbeat")]
    public IActionResult Heartbeat(HelloRequest request)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "inconnu";
        _tracker.Register(request.PlayerName, ip);
        return Ok();
    }

    [HttpGet("online")]
    public IActionResult Online() => Ok(_tracker.GetOnlinePlayers());
}