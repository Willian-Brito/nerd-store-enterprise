using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace NSE.WebAPI.Core.DatabaseFlavor;

public static class ProviderSelector
{
    public static IServiceCollection ConfigureProviderForContext<TContext>(
        this IServiceCollection services,
        (DatabaseType, string) options) where TContext : DbContext
    {
        var (database, connString) = options;
        var provider = ProviderConfiguration<TContext>.Build<TContext>(connString);
        return database switch
        {
            DatabaseType.SqlServer => services.PersistStore<TContext>(provider.SqlServer),
            DatabaseType.MySql => services.PersistStore<TContext>(provider.MySql),
            DatabaseType.Postgre => services.PersistStore<TContext>(provider.Postgre),
            DatabaseType.Sqlite => services.PersistStore<TContext>(provider.Sqlite),

            _ => throw new ArgumentOutOfRangeException(nameof(database), database, null)
        };
    }
    
    public static Action<DbContextOptionsBuilder> WithProviderAutoSelection<TContext>((DatabaseType, string) options)
        where TContext : DbContext
    {
        var (database, connString) = options;
        var provider = ProviderConfiguration<TContext>.Build<TContext>(connString);
        return database switch
        {
            DatabaseType.SqlServer => provider.SqlServer,
            DatabaseType.MySql => provider.MySql,
            DatabaseType.Postgre => provider.Postgre,
            DatabaseType.Sqlite => provider.Sqlite,
            
            _ => throw new ArgumentOutOfRangeException(nameof(database), database, null)
        };
    }
}