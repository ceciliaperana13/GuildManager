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

    
    // Debits the shared common pool (gold or food) and retrieves the new balance.
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

    // Adjusts the shared pool by a signed delta (positive = gain, negative = loss),
    // used after a local turn update (quest rewards, food consumed) to keep
    // the shared balance synchronized and saved for all players in the co-op guild.
    public async Task<(bool Success, ResourcesResponse? Resources, string? Error)> AdjustResourcesAsync(int goldDelta, int foodDelta)
    {
        var response = await _client.PostAsJsonAsync("api/guild/resources/adjust",
            new { GoldDelta = goldDelta, FoodDelta = foodDelta });

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