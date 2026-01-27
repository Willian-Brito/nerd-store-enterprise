using NSE.Catalog.API.Data;
using NSE.WebAPI.Core.Configuration;

namespace NSE.Catalog.API.Configuration;

public static class ApiConfig
{
    public static void AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddApiCoreConfiguration(configuration)
            .WithDbContext<CatalogContext>(configuration);
    }
}