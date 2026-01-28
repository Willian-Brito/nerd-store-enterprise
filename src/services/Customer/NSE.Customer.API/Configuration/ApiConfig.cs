using NSE.Customer.API.Data;
using NSE.WebAPI.Core.Configuration;

namespace NSE.Customer.API.Configuration;

public static class ApiConfig
{
    public static void AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddApiCoreConfiguration(configuration)
            .WithDbContext<CustomerContext>(configuration);
    }
}