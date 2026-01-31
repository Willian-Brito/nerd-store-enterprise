using NSE.Security.Identity.User;
using NSE.ShoppingCart.API.Data;

namespace NSE.ShoppingCart.API.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IAspNetUser, AspNetUser>();
        services.AddScoped<ShoppingCartContext>();
        services.AddScoped<Endpoints.ShoppingCart>();
    }
}