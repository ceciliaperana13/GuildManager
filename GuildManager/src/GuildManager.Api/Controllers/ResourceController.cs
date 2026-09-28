using System.Threading;
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
    // Un seul écrivain à la fois : évite que deux joueurs s'écrasent mutuellement.
    private static readonly SemaphoreSlim _lock = new(1, 1);

    private readonly ISaveFileStore _coopStore;
    private readonly IHubContext<GuildHub> _hub;
    private readonly ILogger<ResourceController> _logger;

    public record TransferResourcesRequest(string ResourceType, int Amount);
    public record AdjustResourcesRequest(int GoldDelta, int FoodDelta);
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

    // Withdraws gold or food from the co-op guild's shared pool.
    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer(TransferResourcesRequest request)
    {
        if (request.Amount <= 0)
            return BadRequest("Le montant doit être positif.");

        await _lock.WaitAsync();
        try
        {
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
        finally
        {
            _lock.Release();
        }
    }

    // Adjusts the shared pool by a signed delta (positive = gain, negative = loss).
    [HttpPost("adjust")]
    public async Task<IActionResult> Adjust(AdjustResourcesRequest request)
    {
        await _lock.WaitAsync();
        try
        {
            var state = await _coopStore.LoadAsync();

            state.Gold = Math.Max(0, state.Gold + request.GoldDelta);
            state.Food = Math.Max(0, state.Food + request.FoodDelta);

            await _coopStore.SaveAsync(state);

            _logger.LogInformation(
                "Ajustement partagé : {GoldDelta} or / {FoodDelta} nourriture (nouveau solde : {Gold} or, {Food} nourriture)",
                request.GoldDelta, request.FoodDelta, state.Gold, state.Food);

            await _hub.Clients.Group(GuildHub.GuildGroup(state.GuildId))
                .SendAsync("ResourcesUpdated", new ResourcesResponse(state.Gold, state.Food));

            return Ok(new ResourcesResponse(state.Gold, state.Food));
        }
        finally
        {
            _lock.Release();
        }
    }

    // Returns the current state of shared resources
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var state = await _coopStore.LoadAsync();
        return Ok(new ResourcesResponse(state.Gold, state.Food));
    }
}