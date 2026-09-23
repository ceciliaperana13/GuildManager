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
    public record HireResponse(int Gold, int Food, object Adventurer);

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

    [HttpPost("hire")]
    public async Task<IActionResult> Hire(HireRequest request)
    {
        // 1. Récupérer l'aventurier proposé au recrutement
        var adventurer = _adventurerManager.adventurersToHire
            .FirstOrDefault(a => a.id == request.AdventurerId);

        if (adventurer is null)
            return NotFound("Aventurier introuvable dans la liste de recrutement.");

        GuildSaveState state;
        try
        {
            // 2. Débit atomique : Load + vérification + Save en une seule section critique.
            //    Si deux joueurs recrutent en même temps, ils sont sérialisés ici,
            //    pas de lost update possible.
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

        // 4. Notifier tous les clients du groupe coop en temps réel
        await _hub.Clients.Group(GuildHub.GuildGroup(state.GuildId))
            .SendAsync("ResourcesUpdated", new { state.Gold, state.Food });

        await _hub.Clients.Group(GuildHub.GuildGroup(state.GuildId))
            .SendAsync("AdventurerHired", adventurer);

        return Ok(new HireResponse(state.Gold, state.Food, adventurer));
    }
}