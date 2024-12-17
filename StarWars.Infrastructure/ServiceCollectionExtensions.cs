using Microsoft.Extensions.DependencyInjection;
using StarWars.Application.Services;
using StarWars.Domain.Services;
using StarWars.Infrastructure.HttpClients;

namespace StarWars.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void AddHttpClients(this IServiceCollection services)
    {
        services.AddTransient<PollyHandler>();
        services.AddHttpClient("StarshipsApi", client =>
            {
                client.BaseAddress = new Uri("https://www.swapi.tech/api/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddHttpMessageHandler<PollyHandler>();
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IStarshipService, StarshipService>();
    }
}