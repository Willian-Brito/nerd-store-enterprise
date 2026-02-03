using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using NSE.Core.Data;
using NSE.Security.Identity.User;

namespace NSE.WebAPI.Core.DatabaseFlavor;

public abstract class BaseDbContext : DbContext
{
    private readonly IAspNetUser _user;

    protected BaseDbContext() { }
    protected BaseDbContext(DbContextOptions options, IAspNetUser user)
        : base(options)
    {
        _user = user;
    }
    
    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default
    )
    {
        if (HasAuditableEntitiesTracked())
            ApplyAuditInfo();
        
        return await base.SaveChangesAsync(cancellationToken);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        
        var configuration = BuildConfiguration();     
        var dbOptions = DatabaseProviderDetector.Detect(configuration);
        var contextType = GetType();
        var method = typeof(ProviderSelector)
            .GetMethod(nameof(ProviderSelector.WithProviderAutoSelection))!
            .MakeGenericMethod(contextType);

        var providerAction =
            (Action<DbContextOptionsBuilder>)method.Invoke(null, new object[] { dbOptions })!;
    
        providerAction(optionsBuilder);
        optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }
    
    private static IConfiguration BuildConfiguration()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }
    
    private bool HasAuditableEntitiesTracked()
    {
        return ChangeTracker.Entries<IAuditableEntity>().Any();
    }
    
    private void ApplyAuditInfo()
    {
        var httpContext = _user?.GetHttpContext();
        var userId = httpContext != null ? _user.GetUserId() : Guid.Empty;

        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(x => x.CreatedBy).CurrentValue = userId;
                    entry.Property(x => x.CreatedAt).CurrentValue = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Property(x => x.UpdatedBy).CurrentValue = userId;
                    entry.Property(x => x.UpdatedAt).CurrentValue = DateTime.UtcNow;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Property(x => x.DeletedBy).CurrentValue = userId;
                    entry.Property(x => x.DeletedAt).CurrentValue = DateTime.UtcNow;
                    break;
            }
        }
    }
}