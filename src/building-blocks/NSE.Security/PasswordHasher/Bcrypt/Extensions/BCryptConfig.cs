using Microsoft.AspNetCore.Identity;
using NSE.Security.PasswordHasher.Core;
using NSE.Security.PasswordHasher.Bcrypt;

namespace Microsoft.Extensions.DependencyInjection;

public static class BCryptConfig
{
    /// <summary>
    /// Use Bcrypt password hashing algorithm.
    /// </summary>
    public static IServiceCollection UseBcrypt<TUser>(this IPasswordHashBuilder builder) where TUser : class
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
        return builder.Services.AddScoped<IPasswordHasher<TUser>, BCrypt<TUser>>();
    }
}