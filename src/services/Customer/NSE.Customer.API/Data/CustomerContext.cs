using Microsoft.EntityFrameworkCore;
using NSE.Core.Bus;
using NSE.Core.Data;
using NSE.Core.Messages.Base;
using NSE.Security.Identity.User;
using NSE.WebAPI.Core.DatabaseFlavor;
using Entities = NSE.Customer.API.Models.Entities;
using FluentValidation.Results;
using NSE.WebAPI.Core.Extensions;

namespace NSE.Customer.API.Data;

public sealed class CustomerContext : BaseDbContext, IUnitOfWork
{
    private readonly IMessageBus _messageBus;
    public DbSet<Entities.Customer> Customers { get; set; }
    public DbSet<Entities.Address> Addresses { get; set; }

    public CustomerContext() { }
    
    public CustomerContext(
        DbContextOptions<CustomerContext> options, 
        IAspNetUser user,
        IMessageBus messageBus
    ) : base(options, user)
    {
        _messageBus = messageBus;
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        ChangeTracker.AutoDetectChangesEnabled = false;
    }
    public async Task<bool> Commit()
    {
        var success = await SaveChangesAsync() > 0;
        if (success) await _messageBus.PublishEvents(this);

        return success;
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<ValidationResult>();
        modelBuilder.Ignore<Event>();

        foreach (var property in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
            property.SetColumnType("varchar(100)");

        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(e => e.GetForeignKeys())) relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomerContext).Assembly);
    }
}