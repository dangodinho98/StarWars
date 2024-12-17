using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarWars.Application.Exceptions;
using StarWars.Domain.Models.Starship;
using StarWars.Domain.Models.ViewModels;
using StarWars.Domain.Services;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace StarWars.Web.Controllers;

[Authorize]
public class HomeController(IStarshipService starshipService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(string? selectedManufacturer = null, int page = 1, int limit = 10)
    {
        try
        {
            var allStarships = await starshipService.GetAllStarshipsAsync();
            var allManufacturers = GetAllManufacturers(allStarships);

            var filteredStarships = allStarships;
            if (!string.IsNullOrEmpty(selectedManufacturer))
            {
                filteredStarships = filteredStarships
                    .Where(s => s.Manufacturer.Contains(selectedManufacturer, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var totalRecords = filteredStarships.Count;
            var totalPages = (int)Math.Ceiling((double)totalRecords / limit);
            var paginatedStarships = filteredStarships
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();

            ViewData["Manufacturers"] = allManufacturers;
            ViewData["SelectedManufacturer"] = selectedManufacturer;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;

            return View(paginatedStarships);
        }
        catch (ApiException ex)
        {
            ViewData["ErrorMessage"] = ex.Message;
            return View(Array.Empty<Starship>());
        }
    }

    private static List<string> GetAllManufacturers(List<Starship> allStarships)
    {
        // Regex pattern to avoid splitting on ", Inc.", ", Corp.", etc.
        const string pattern = @"(?:, )(?!(Inc\.|Corp\.|Ltd\.|Incorporated|LLC|Systems))";

        // Reserved words that should not appear as standalone results
        var reservedWords = new HashSet<string> { "Inc.", "Inc", "Corp.", "Ltd.", "Incorporated", "LLC", "Systems" };

        // Step 1: Extract distinct manufacturers
        var distinctManufacturers = allStarships
            .Select(s => s.Manufacturer)
            .Where(m => !string.IsNullOrWhiteSpace(m))
            .Distinct();

        // Step 2: Process each manufacturer
        var result = distinctManufacturers
            .SelectMany(manufacturer =>
                manufacturer.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)) // Split on slashes first
            .SelectMany(part => Regex.Split(part, pattern)) // Then split on commas safely
            .Select(part => part.Trim()) // Trim whitespace
            .Where(part => !string.IsNullOrEmpty(part) && !reservedWords.Contains(part)) // Exclude empty and reserved words
            .Distinct()
            .Order()
            .ToList();

        return result;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
