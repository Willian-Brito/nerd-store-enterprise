using NSE.Payment.API.Data;
using NSE.Payment.API.Facade;
using NSE.WebAPI.Core.Configuration;

namespace NSE.Payment.API.Configuration;

public static class ApiConfig
{
    public static void AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApiCoreConfiguration(configuration)
            .WithDbContext<PaymentContext>(configuration)
            .Configure<PaymentConfig>(configuration.GetSection("PaymentConfig"));
    }
}