using GuildManager.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace GuildManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly GuildManagerDbContext _context;

    public HealthController(GuildManagerDbContext context)
    {
        _context = context;
    }

    // GET /api/health
    [HttpGet]
    public IActionResult Ping() => Ok(new { status = "api_ok" });

    // GET /api/health/db  -> teste la connexion réelle à PostgreSQL
    [HttpGet("db")]
    public async Task<IActionResult> CheckDatabase()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            if (!canConnect)
                return StatusCode(503, new { status = "unreachable" });

            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();

            return Ok(new
            {
                status = "ok",
                database = _context.Database.GetDbConnection().Database,
                pendingMigrations
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { status = "error", message = ex.Message });
        }
    }
}
