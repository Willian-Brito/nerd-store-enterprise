using NSE.Bff.Checkout.Services.gRPC;
using NSE.ShoppingCart.API.Services.gRPC;
using NSE.WebAPI.Core.Extensions;

namespace NSE.Bff.Checkout.Configuration;

public static class GrpcConfig
{
    public static void ConfigureGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<GrpcServiceInterceptor>();
        services.AddScoped<IShoppingCartGrpcService, ShoppingCartGrpcService>();
        
        services.AddGrpcClient<ShoppingCartOrders.ShoppingCartOrdersClient>(options =>
            {
                options.Address = new Uri(configuration["ShoppingCartUrl"]!);
            })
            .AddInterceptor<GrpcServiceInterceptor>()
            .AllowSelfSignedCertificate();
    }
}