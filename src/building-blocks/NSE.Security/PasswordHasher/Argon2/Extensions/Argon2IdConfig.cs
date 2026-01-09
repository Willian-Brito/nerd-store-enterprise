using Microsoft.AspNetCore.Identity;
using NSE.Security.PasswordHasher.Argon2;
using NSE.Security.PasswordHasher.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class Argon2IdConfig
{
    public static IServiceCollection UseArgon2<TUser>(this IPasswordHashBuilder builder) where TUser : class
    {
        builder.Services.Configure<ImprovedPasswordHasherOptions>(options =>
        {
            options.Strength = builder.Options.Strength;
            options.MemLimit = builder.Options.MemLimit;
            options.OpsLimit = builder.Options.OpsLimit;
            options.WorkFactor = builder.Options.WorkFactor;
            options.SaltRevision = builder.Options.SaltRevision;
        });
        builder.Services.AddScoped<PasswordHasher<TUser>>();
        return builder.Services.AddScoped<IPasswordHasher<TUser>, Argon2Id<TUser>>();
    }
}