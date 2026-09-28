using GuildManager.Api.Hubs;
using GuildManager.Api.Models;
using GuildManager.Api.Services;
using GuildManager.Aplication.Guilds.Controls;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GuildManager.Api.Controllers;

[ApiController]
[Route("api/guild/adventurers")]
public class AdventurerController : ControllerBase
{
    private readonly ISaveFileStore _coopStore;
    private readonly IHubContext<GuildHub> _hub;
    private readonly AdventurerManager _adventurerManager;
    private readonly ILogger<AdventurerController> _logger;

    public record HireRequest(int AdventurerId);
    public record HireResponse(int Gold, int Food, AdventurerCandidateDto Adventurer);

    public AdventurerController(
        [FromKeyedServices("coop")] ISaveFileStore coopStore,
        IHubContext<GuildHub> hub,
        AdventurerManager adventurerManager,
        ILogger<AdventurerController> logger)
    {
        _coopStore = coopStore;
        _hub = hub;
        _adventurerManager = adventurerManager;
        _logger = logger;
    }

    // Complete roster of adventurers already recruited by the guild (as opposed 
    //  to candidates available for recruitment via /to-hire). This is the list 
    //  that the client should use to select participants for a co-op quest.
    [HttpGet]
    public IActionResult GetRoster()
    {
        var dtos = _adventurerManager.adventurers.Select(ToDto).ToList();
        return Ok(dtos);
    }

   // List of candidates currently available for recruitment by the co-op guild.
   // Automatically regenerated if empty (e.g. immediately after server startup).²
    [HttpGet("to-hire")]
    public async Task<IActionResult> GetToHire()
    {
        if (_adventurerManager.adventurersToHire.Count == 0)
        {
            var state = await _coopStore.LoadAsync();
            _adventurerManager.refreshadventurersToHire(Math.Max(state.CurrentTurn > 0 ? 1 : 1, 1));
            // NOTE: prestige is not tracked in GuildSaveState yet
            // For now, generate with prestige = 1 by default.
        }
        var dtos = _adventurerManager.adventurersToHire.Select(ToDto).ToList();
        return Ok(dtos);
    }

    [HttpPost("hire")]
    public async Task<IActionResult> Hire(HireRequest request)
    {
        var adventurer = _adventurerManager.adventurersToHire
            .FirstOrDefault(a => a.id == request.AdventurerId);

        if (adventurer is null)
            return NotFound("Aventurier introuvable dans la liste de recrutement (elle a peut-être été rafraîchie entre-temps).");

        GuildSaveState state;
        try
        {
            state = await _coopStore.UpdateAsync(s =>
            {
                if (s.Gold < adventurer.goldPrice)
                    throw new InsufficientResourcesException("Pas assez d'or dans le pot commun.");
                if (s.Food < adventurer.foodPrice)
                    throw new InsufficientResourcesException("Pas assez de nourriture dans le pot commun.");

                s.Gold -= adventurer.goldPrice;
                s.Food -= adventurer.foodPrice;
            });
        }
        catch (InsufficientResourcesException ex)
        {
            return BadRequest(ex.Message);
        }

        _adventurerManager.AddAdventurer(adventurer);
        _adventurerManager.adventurersToHire.Remove(adventurer);

        _logger.LogInformation(
            "Aventurier {Name} (id {Id}) recruté pour {Gold} or / {Food} nourriture",
            adventurer.name, adventurer.id, adventurer.goldPrice, adventurer.foodPrice);

        await _hub.Clients.Group(GuildHub.GuildGroup(state.GuildId))
            .SendAsync("ResourcesUpdated", new { state.Gold, state.Food });

        await _hub.Clients.Group(GuildHub.GuildGroup(state.GuildId))
            .SendAsync("AdventurerHired", ToDto(adventurer));

        return Ok(new HireResponse(state.Gold, state.Food, ToDto(adventurer)));
    }

    private static AdventurerCandidateDto ToDto(Adventurer a) => new(
        a.id, a.name, a.job, a.lvl, a.xp, a.health, a.def,
        a.magicAttack, a.physicAttack, a.image, a.debuff,
        a.isHurted, a.hurtTurn, a.isDead, a.isInQuest,
        a.goldPrice, a.foodPrice);
}