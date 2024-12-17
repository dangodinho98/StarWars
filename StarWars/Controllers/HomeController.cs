using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarWars.Application.Exceptions;
using StarWars.Domain.Models.Starship;
using StarWars.Domain.Services;
using StarWars.Models;
using System.Diagnostics;

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
            var allManufacturers = allStarships
                .Select(s => s.Manufacturer)
                .Where(m => !string.IsNullOrEmpty(m))
                .Distinct()
                .OrderBy(m => m)
                .ToList();

            var filteredStarships = allStarships;
            if (!string.IsNullOrEmpty(selectedManufacturer))
            {
                filteredStarships = filteredStarships
                    .Where(s => selectedManufacturer.Equals(s.Manufacturer, StringComparison.OrdinalIgnoreCase))
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
