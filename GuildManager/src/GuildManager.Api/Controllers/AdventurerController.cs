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

    // Roster complet des aventuriers déjà recrutés par la guilde (par opposition
    // aux candidats au recrutement de /to-hire). C'est cette liste que le client
    // doit utiliser pour la sélection des participants à une quête en coop.
    [HttpGet]
    public IActionResult GetRoster()
    {
        var dtos = _adventurerManager.adventurers.Select(ToDto).ToList();
        return Ok(dtos);
    }

    // Liste des candidats actuellement proposés au recrutement pour la guilde coop.
    // Régénérée automatiquement si elle est vide (ex: juste après le démarrage du serveur).
    [HttpGet("to-hire")]
    public async Task<IActionResult> GetToHire()
    {
        if (_adventurerManager.adventurersToHire.Count == 0)
        {
            var state = await _coopStore.LoadAsync();
            _adventurerManager.refreshadventurersToHire(Math.Max(state.CurrentTurn > 0 ? 1 : 1, 1));
            // NB: prestige n'est pas encore suivi côté GuildSaveState.
            // En attendant, on génère avec prestige = 1 par défaut.
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
        a.id, a.name, a.job, a.lvl, a.health, a.def,
        a.magicAttack, a.physicAttack, a.image, a.goldPrice, a.foodPrice);
}