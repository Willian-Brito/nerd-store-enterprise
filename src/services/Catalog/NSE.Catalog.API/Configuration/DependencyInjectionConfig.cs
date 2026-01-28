using NSE.Catalog.API.Data;
using NSE.Catalog.API.Data.Models.Interfaces;
using NSE.Catalog.API.Data.Repository;

namespace NSE.Catalog.API.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<CatalogContext>();
    }
}