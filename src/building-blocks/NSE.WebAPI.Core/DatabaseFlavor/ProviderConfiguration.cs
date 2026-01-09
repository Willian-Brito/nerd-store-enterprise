using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace NSE.WebAPI.Core.DatabaseFlavor;

public class ProviderConfiguration<TContext> where TContext : DbContext
{
    private readonly string _connectionString;
    
    private static readonly string MigrationAssembly =
        typeof(TContext).Assembly.GetName().Name;
    
    public ProviderConfiguration(string connString)
    {
        _connectionString = connString;
    }
    
    public Action<DbContextOptionsBuilder> SqlServer =>
        options => options.UseSqlServer(_connectionString, sql => sql.MigrationsAssembly(MigrationAssembly));

    public Action<DbContextOptionsBuilder> MySql =>
        options => options.UseMySQL(_connectionString, sql => sql.MigrationsAssembly(MigrationAssembly));

    public Action<DbContextOptionsBuilder> Postgre =>
        options =>
        {
            options.UseNpgsql(_connectionString, sql => sql.MigrationsAssembly(MigrationAssembly));
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        };

    public Action<DbContextOptionsBuilder> Sqlite =>
        options => options.UseSqlite(_connectionString, sql => sql.MigrationsAssembly(MigrationAssembly));
    
    public static ProviderConfiguration<TContext> Build<TContext>(string connString)
        where TContext : DbContext
    {
        return new ProviderConfiguration<TContext>(connString);
    }
}