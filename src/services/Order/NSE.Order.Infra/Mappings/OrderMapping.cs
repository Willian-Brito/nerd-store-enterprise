using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Entities = NSE.Order.Domain.Entities.Orders;

namespace NSE.Order.Infra.Mappings;

public class OrderMapping : IEntityTypeConfiguration<Entities.Order>
{
    public void Configure(EntityTypeBuilder<Entities.Order> builder)
    {
        builder.HasKey(c => c.Id);

        builder.OwnsOne(p => p.Address, e =>
        {
            e.Property(pe => pe.StreetAddress)
                .HasColumnName("StreetAddress");

            e.Property(pe => pe.BuildingNumber)
                .HasColumnName("BuildingNumber");

            e.Property(pe => pe.SecondaryAddress)
                .HasColumnName("SecondaryAddress");

            e.Property(pe => pe.Neighborhood)
                .HasColumnName("Neighborhood");

            e.Property(pe => pe.ZipCode)
                .HasColumnName("ZipCode");

            e.Property(pe => pe.City)
                .HasColumnName("City");

            e.Property(pe => pe.State)
                .HasColumnName("State");
        });

        builder.Property(c => c.Code).UseIdentityColumn(100000);

        // 1 : N => Order : OrderItems
        builder.HasMany(c => c.OrderItems)
            .WithOne(c => c.Order)
            .HasForeignKey(c => c.OrderId);

        builder.ToTable("Orders");
    }
}