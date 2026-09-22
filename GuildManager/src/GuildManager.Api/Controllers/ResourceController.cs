using GuildManager.Api.Hubs;
using GuildManager.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GuildManager.Api.Controllers;

[ApiController]
[Route("api/guild/resources")]
public class ResourceController : ControllerBase
{
    private readonly ISaveFileStore _coopStore;
    private readonly IHubContext<GuildHub> _hub;
    private readonly ILogger<ResourceController> _logger;

    public record TransferResourcesRequest(string ResourceType, int Amount);
    public record ResourcesResponse(int Gold, int Food);

    public ResourceController(
        [FromKeyedServices("coop")] ISaveFileStore coopStore,
        IHubContext<GuildHub> hub,
        ILogger<ResourceController> logger)
    {
        _coopStore = coopStore;
        _hub = hub;
        _logger = logger;
    }


    /// Retire du gold ou de la nourriture du pot commun de la guilde coop.
    /// Diffuse le nouvel état en temps réel via SignalR.
    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer(TransferResourcesRequest request)
    {
        if (request.Amount <= 0)
            return BadRequest("Le montant doit être positif.");

        var state = await _coopStore.LoadAsync();

        switch (request.ResourceType?.ToLowerInvariant())
        {
            case "gold":
                if (state.Gold < request.Amount)
                    return BadRequest("Pas assez d'or dans le pot commun.");
                state.Gold -= request.Amount;
                break;

            case "food":
                if (state.Food < request.Amount)
                    return BadRequest("Pas assez de nourriture dans le pot commun.");
                state.Food -= request.Amount;
                break;

            default:
                return BadRequest("Type de ressource invalide (attendu : \"gold\" ou \"food\").");
        }

        await _coopStore.SaveAsync(state);

        _logger.LogInformation(
            "Ressources partagées : {Amount} {Type} retirés du pot commun",
            request.Amount, request.ResourceType);

        await _hub.Clients.Group(GuildHub.GuildGroup(state.GuildId))
            .SendAsync("ResourcesUpdated", new ResourcesResponse(state.Gold, state.Food));

        return Ok(new ResourcesResponse(state.Gold, state.Food));
    }

    //Retourne l'état actuel des ressources partagées
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var state = await _coopStore.LoadAsync();
        return Ok(new ResourcesResponse(state.Gold, state.Food));
    }
}