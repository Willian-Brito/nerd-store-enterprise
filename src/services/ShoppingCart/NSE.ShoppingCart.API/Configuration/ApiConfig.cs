using NSE.ShoppingCart.API.Data;
using NSE.ShoppingCart.API.Services.gRPC;
using NSE.WebAPI.Core.Configuration;

namespace NSE.ShoppingCart.API.Configuration;

public static class ApiConfig
{
    public static void AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApiCoreConfiguration(configuration)
            .WithDbContext<ShoppingCartContext>(configuration)
            .AddGrpc();
    }

    public static void UseApiConfiguration(this WebApplication app, IWebHostEnvironment env)
    {
        app.UseApiCoreConfiguration(env);

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGrpcService<ShoppingCartGrpcService>().RequireCors("Total");
    }
}