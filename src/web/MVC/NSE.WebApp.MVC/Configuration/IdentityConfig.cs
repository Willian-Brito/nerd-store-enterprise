using Microsoft.AspNetCore.Authentication.Cookies;

namespace NSE.WebApp.MVC.Configuration;

public static class AuthConfig
{
    public static void AddAuthConfiguration(this IServiceCollection services)
    {
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                options.LoginPath = "/login";
                options.AccessDeniedPath = "/erro/403";
            });

        services.AddHttpContextAccessor();
    }

    public static void UseAuthConfiguration(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}