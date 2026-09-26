using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GuildManager.Client.Services;

public record ResourcesResponse(int Gold, int Food);

public class GuildApiClient
{
    private readonly HttpClient _client;

    public GuildApiClient()
    {
        _client = new HttpClient { BaseAddress = new Uri(AppSession.ApiBaseUrl) };
    }

    // Débite le pot commun partagé (or ou nourriture) et récupère le nouveau solde.
    public async Task<(bool Success, ResourcesResponse? Resources, string? Error)> TransferResourceAsync(string resourceType, int amount)
    {
        var response = await _client.PostAsJsonAsync("api/guild/resources/transfer",
            new { ResourceType = resourceType, Amount = amount });

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return (false, null, error);
        }

        var result = await response.Content.ReadFromJsonAsync<ResourcesResponse>();
        return (true, result, null);
    }

    public async Task<ResourcesResponse?> GetResourcesAsync()
        => await _client.GetFromJsonAsync<ResourcesResponse>("api/guild/resources");
}