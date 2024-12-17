namespace StarWars.Domain.Services;

using StarWars.Domain.Models.Starship;
using System.Threading.Tasks;

public interface IStarshipService
{
    Task<List<Starship>> GetStarshipsByManufacturerAsync(string? selectedManufacturer);
    Task<List<Starship>> GetAllStarshipsAsync();
}
