using Microsoft.EntityFrameworkCore;
using NSE.Core.Data;
using NSE.Core.Messages.Base;
using FluentValidation.Results;
using NSE.Catalog.API.Models;
using NSE.Security.Identity.User;
using NSE.WebAPI.Core.DatabaseFlavor;

namespace NSE.Catalog.API.Data;

public class CatalogContext : BaseDbContext, IUnitOfWork
{
    public DbSet<Product> Products { get; set; }
    
    public CatalogContext(){ }
    public CatalogContext(DbContextOptions<CatalogContext> options, IAspNetUser user)
        :base(options, user) { }
    
    public async Task<bool> Commit()
    {
        return await base.SaveChangesAsync() > 0;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<ValidationResult>();
        modelBuilder.Ignore<Event>();

        foreach (var property in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
            property.SetColumnType("varchar(100)");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogContext).Assembly);
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
}