using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSE.Core.Bus;
using NSE.Core.Data;
using NSE.Core.Messages.Base;
using NSE.Order.Domain.Entities.Orders;
using NSE.Order.Domain.Entities.Vouchers;
using NSE.Security.Identity.User;
using NSE.WebAPI.Core.DatabaseFlavor;
using NSE.WebAPI.Core.Extensions;

namespace NSE.Order.Infra.Context;

public class OrdersContext : BaseDbContext, IUnitOfWork
{
    private readonly IConfiguration _configuration;
    private readonly IMessageBus _messageBus;
    
    public DbSet<Domain.Entities.Orders.Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Voucher> Vouchers { get; set; }

    public OrdersContext(
        DbContextOptions<OrdersContext> options, 
        IMessageBus messageBus,
        IConfiguration configuration,
        IAspNetUser user
    ) : base(options, user)
    {
        _messageBus = messageBus;
        _configuration = configuration;
    }
    
    public async Task<bool> Commit()
    {
        var sucess = await base.SaveChangesAsync() > 0;
        if (sucess) await _messageBus.PublishEvents(this);

        return sucess;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var property in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
            property.SetColumnType("varchar(100)");

        modelBuilder.Ignore<Event>();
        modelBuilder.Ignore<ValidationResult>();
        modelBuilder.Entity<Domain.Entities.Orders.Order>().Property(p => p.Code).HasIdentityOptions(1000);

        if (_configuration["AppSettings:DatabaseType"] == "Postgre")
        {
            modelBuilder.UseIdentityAlwaysColumns();
            modelBuilder.Entity<Domain.Entities.Orders.Order>().Property(p => p.Code).UseIdentityAlwaysColumn();
        }

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersContext).Assembly);

        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(e => e.GetForeignKeys())) relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

        base.OnModelCreating(modelBuilder);
    }
}