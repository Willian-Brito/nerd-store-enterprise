using NSE.Payment.API.Data;
using NSE.WebAPI.Core.Configuration;

namespace NSE.Payment.API.Configuration;

public static class DbMigrationHelpers
{
    /// <summary>
    ///     Generate migrations before running this method, you can use command bellow:
    ///     Nuget package manager: Add-Migration DbInit -context BillingContext
    ///     Dotnet CLI: dotnet ef migrations add DbInit -c BillingContext
    /// </summary>
    public static async Task EnsureSeedData(WebApplication serviceScope)
    {
        var services = serviceScope.Services.CreateScope().ServiceProvider;
        await EnsureSeedData(services);
    }

    private static async Task EnsureSeedData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

        var ssoContext = scope.ServiceProvider.GetRequiredService<PaymentContext>();

        if (env.IsDevelopment() || env.IsEnvironment("Docker"))
            await ssoContext.Database.EnsureCreatedAsync();

        await DbHealthChecker.TestConnection(ssoContext);
    }
}