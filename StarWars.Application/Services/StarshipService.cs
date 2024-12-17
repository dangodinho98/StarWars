using Microsoft.Extensions.Caching.Memory;
using StarWars.Application.Exceptions;
using StarWars.Domain.Models.Starship;
using StarWars.Domain.Services;
using System.Text.Json;

namespace StarWars.Application.Services;

/// <summary>
/// Service for fetching starship data and manufacturers from the SWAPI.
/// </summary>
public class StarshipService(
    IHttpClientFactory httpClientFactory,
    IMemoryCache cache)
    : IStarshipService
{
    private const string StarshipCacheKey = "Starships";

    /// <summary>
    /// Gets all starships, using the cache if available.
    /// </summary>
    public async Task<List<Starship>> GetAllStarshipsAsync()
    {
        if (cache.TryGetValue(StarshipCacheKey, out List<Starship>? starships))
        {
            return starships ?? [];
        }

        starships = new List<Starship>();
        var client = httpClientFactory.CreateClient("StarshipsApi");

        try
        {
            var starshipIds = await FetchStarshipIdsAsync(client, starships);
            await FetchDetailedStarshipsAsync(client, starshipIds, starships);

            cache.Set(StarshipCacheKey, starships, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(10)
            });
        }
        catch (Exception ex)
        {
            throw new ApiException($"An error occurred while fetching starship data: {ex.Message}", ex);
        }

        return starships;
    }

    /// <summary>
    /// Filters the cached starships by the selected manufacturer.
    /// </summary>
    public async Task<List<Starship>> GetStarshipsByManufacturerAsync(string? selectedManufacturer)
    {
        var starships = await GetAllStarshipsAsync();

        if (!string.IsNullOrEmpty(selectedManufacturer))
        {
            starships = starships
                .Where(s => selectedManufacturer.Equals(s.Manufacturer, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return starships;
    }

    /// <summary>
    /// Fetches starship IDs and populates the basic starship list with limited data.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> instance.</param>
    /// <param name="starships">The list to populate with basic starship data.</param>
    /// <returns>A list of starship IDs.</returns>
    private static async Task<List<string>> FetchStarshipIdsAsync(HttpClient client, List<Starship> starships)
    {
        var starshipIds = new List<string>();
        var page = 1;

        while (true)
        {
            var response = await FetchStarshipDataAsync(client, page, 100);
            foreach (var result in response.RootElement.GetProperty("results").EnumerateArray())
            {
                var uid = result.GetProperty("uid").GetString();
                var name = result.GetProperty("name").GetString();
                var url = result.GetProperty("url").GetString();

                if (!string.IsNullOrEmpty(uid))
                {
                    starshipIds.Add(uid);
                    starships.Add(new Starship
                    {
                        Uid = uid,
                        Name = name,
                        Url = url
                    });
                }
            }

            var totalPages = response.RootElement.GetProperty("total_pages").GetInt32();
            if (page >= totalPages) break;
            page++;
        }

        return starshipIds;
    }

    /// <summary>
    /// Fetches detailed starship data and populates manufacturer information.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> instance.</param>
    /// <param name="starshipIds">The list of starship IDs to fetch details for.</param>
    /// <param name="starships">The list of starships to enrich with detailed data.</param>
    private static async Task FetchDetailedStarshipsAsync(HttpClient client, List<string> starshipIds, List<Starship> starships)
    {
        var tasks = starshipIds.Select(uid => FetchStarshipDetailsAsync(client, uid));
        var starshipDetails = await Task.WhenAll(tasks);

        for (var i = 0; i < starships.Count; i++)
        {
            var starshipDetail = starshipDetails[i];
            if (!starshipDetail.TryGetProperty("manufacturer", out var manufacturerProperty)) continue;

            var manufacturer = manufacturerProperty.GetString();
            if (!string.IsNullOrEmpty(manufacturer))
            {
                starships[i].Manufacturer = manufacturer;
            }
        }
    }

    /// <summary>
    /// Fetches paginated starship data.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> instance.</param>
    /// <param name="page">The page number to fetch.</param>
    /// <param name="limit">The number of items per page.</param>
    /// <returns>A <see cref="JsonDocument"/> containing the raw JSON response.</returns>
    private static async Task<JsonDocument> FetchStarshipDataAsync(HttpClient client, int page, int limit)
    {
        var response = await client.GetAsync($"starships?page={page}&limit={limit}");
        if (!response.IsSuccessStatusCode)
        {
            throw new ApiException($"Failed to fetch starship data. Status Code: {response.StatusCode}");
        }

        var content = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(content);
    }

    /// <summary>
    /// Fetches detailed starship data for a specific starship ID.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> instance.</param>
    /// <param name="uid">The unique ID of the starship.</param>
    /// <returns>A <see cref="JsonElement"/> containing the detailed starship data.</returns>
    private static async Task<JsonElement> FetchStarshipDetailsAsync(HttpClient client, string uid)
    {
        var response = await client.GetAsync($"starships/{uid}");
        if (!response.IsSuccessStatusCode)
        {
            throw new ApiException($"Failed to fetch starship details for ID {uid}. Status Code: {response.StatusCode}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(content);

        return jsonDocument.RootElement.GetProperty("result").GetProperty("properties");
    }
}