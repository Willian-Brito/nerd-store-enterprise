using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using NSE.Core.Data;
using NSE.Core.Messages.Base;
using NSE.Security.Identity.User;
using NSE.WebAPI.Core.DatabaseFlavor;
using Models = NSE.Payment.API.Models;

namespace NSE.Payment.API.Data;

public sealed class PaymentContext : BaseDbContext, IUnitOfWork
{
    public DbSet<Models.Payment> Payments { get; set; }
    public DbSet<Models.Transaction> Transactions { get; set; }
    
    public PaymentContext() { }

    public PaymentContext(
        DbContextOptions<PaymentContext> options,
        IAspNetUser user
    ) : base (options, user)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        ChangeTracker.AutoDetectChangesEnabled = false;
    }
    
    public async Task<bool> Commit()
    {
        return await SaveChangesAsync() > 0;
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

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentContext).Assembly);
    }
}