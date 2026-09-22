using GuildManager.Api.Services;
using GuildManager.Domain.Entities;
using GuildManager.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GuildManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly GuildManagerDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<AuthController> _logger;

    public AuthController(GuildManagerDbContext context, IPasswordHasher passwordHasher, ILogger<AuthController> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public record RegisterRequest(string Username, string Email, string Password);
    public record LoginRequest(string Username, string Password);
    public record UserResponse(int Id, string Username, string Email);

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Nom d'utilisateur, email et mot de passe sont requis.");
        }

        var username = request.Username.Trim();
        var email = request.Email.Trim();

        var exists = await _context.Users.AnyAsync(u =>
            u.Username == username || u.Email == email);

        if (exists)
            return Conflict("Ce nom d'utilisateur ou cet email est déjà utilisé.");

        var (hash, salt) = _passwordHasher.Hash(request.Password);

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = hash,
            Salt = salt,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Users.Add(user);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // security 
            _logger.LogWarning(ex, "Conflit lors de l'inscription de {Username}", username);
            return Conflict("Ce nom d'utilisateur ou cet email est déjà utilisé.");
        }

        _logger.LogInformation("Nouvel utilisateur inscrit : {Username}", username);

        return Ok(new UserResponse(user.Id, user.Username, user.Email));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Nom d'utilisateur et mot de passe requis.");

        var username = request.Username.Trim();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (user is null || !user.IsActive)
            return Unauthorized();

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash, user.Salt))
            return Unauthorized();

        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Connexion réussie : {Username}", username);

        return Ok(new UserResponse(user.Id, user.Username, user.Email));
    }
}