using Microsoft.AspNetCore.Identity;
using NSE.Identity.API.Data;
using NSE.Security.Identity.Jwt;
using NSE.Security.PasswordHasher.Core;
using NSE.WebAPI.Core.DatabaseFlavor;

namespace NSE.Identity.API.Configuration;

public static class IdentityConfig
{
    public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseOptions = DatabaseProviderDetector.Detect(configuration);
        services.ConfigureProviderForContext<ApplicationDbContext>(databaseOptions);

        services.AddMemoryCache().AddDataProtection();
        
        services
            .AddJwtConfiguration(configuration, "AppSettings")
            .AddSecurityIdentity<IdentityUser>()
            .PersistKeysToDatabaseStore<ApplicationDbContext>();

        services.AddIdentity<IdentityUser, IdentityRole>(o =>
            {
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireUppercase = false;
                o.Password.RequiredUniqueChars = 0;
                o.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services
            .UpgradePasswordSecurity()
            .WithStrengthen(PasswordHasherStrength.Moderate)
            .UseArgon2<IdentityUser>();

        return services;
    }
}