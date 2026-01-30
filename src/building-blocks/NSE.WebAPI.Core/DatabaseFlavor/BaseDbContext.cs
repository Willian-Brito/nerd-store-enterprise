using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
        if(!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddJsonFile("appsettings.Production.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            
            var connectionString = configuration.GetConnectionString("DefaultConnection");
    
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseNpgsql(connectionString);
        }        
    }
    
    private bool HasAuditableEntitiesTracked()
    {
        return ChangeTracker.Entries<IAuditableEntity>().Any();
    }
    
    private void ApplyAuditInfo()
    {
        var userId = _user?.GetHttpContext() != null ? _user.GetUserId() : Guid.Empty;

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