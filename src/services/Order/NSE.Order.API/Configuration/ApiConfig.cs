using NSE.Order.Infra.Context;
using NSE.WebAPI.Core.Configuration;

namespace NSE.Order.API.Configuration;

public static class ApiConfig
{
    public static void AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApiCoreConfiguration(configuration)
            .WithDbContext<OrdersContext>(configuration);
    }
}