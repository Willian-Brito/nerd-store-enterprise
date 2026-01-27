using NSE.Catalog.API.Data;
using NSE.Catalog.API.Data.Repository.Products;

namespace NSE.Catalog.API.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<CatalogContext>();
    }
}