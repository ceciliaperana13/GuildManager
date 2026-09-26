using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GuildManager.Client.Services;

public record ResourcesResponse(int Gold, int Food);

public record AdventurerCandidateDto(
    int Id,
    string Name,
    string Job,
    int Lvl,
    int Xp,
    int Health,
    int Def,
    int MagicAttack,
    int PhysicAttack,
    string Image,
    List<string> Debuff,
    bool IsHurted,
    int HurtTurn,
    bool IsDead,
    bool IsInQuest,
    int GoldPrice,
    int FoodPrice);

public class GuildApiClient
{
    private readonly HttpClient _client;

    public GuildApiClient()
    {
        _client = new HttpClient { BaseAddress = new Uri(AppSession.ApiBaseUrl) };
    }

    // Candidats au recrutement (pas encore dans la guilde).
    public async Task<List<AdventurerCandidateDto>> GetAdventurersToHireAsync()
        => await _client.GetFromJsonAsync<List<AdventurerCandidateDto>>("api/guild/adventurers/to-hire")
           ?? new List<AdventurerCandidateDto>();

    // Roster complet des aventuriers déjà recrutés par la guilde coop.
    public async Task<List<AdventurerCandidateDto>> GetAdventurersRosterAsync()
        => await _client.GetFromJsonAsync<List<AdventurerCandidateDto>>("api/guild/adventurers")
           ?? new List<AdventurerCandidateDto>();

    public async Task<(bool Success, ResourcesResponse? Resources, string? Error)> HireAdventurerAsync(int adventurerId)
    {
        var response = await _client.PostAsJsonAsync("api/guild/adventurers/hire",
            new { AdventurerId = adventurerId });

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