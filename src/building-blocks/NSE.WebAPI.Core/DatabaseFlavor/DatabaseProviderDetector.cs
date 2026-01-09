using Microsoft.Extensions.Configuration;

namespace NSE.WebAPI.Core.DatabaseFlavor;

public static class DatabaseProviderDetector
{
    /// <summary>
    /// it's just a tuple. Returns 2 parameters.
    /// Trying to improve readability at ConfigureServices
    /// </summary>
    public static (DatabaseType, string) Detect(IConfiguration configuration) => (
        configuration.GetValue<DatabaseType>("AppSettings:DatabaseType", DatabaseType.None),
        configuration.GetConnectionString("DefaultConnection"));
}