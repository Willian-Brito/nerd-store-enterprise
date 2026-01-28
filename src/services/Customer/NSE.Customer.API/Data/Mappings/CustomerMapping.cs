using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSE.Core.DomainObjects;
using Entities = NSE.Customer.API.Models.Entities;

namespace NSE.Customer.API.Data.Mappings;

public class CustomerMapping : IEntityTypeConfiguration<Entities.Customer>
{
    public void Configure(EntityTypeBuilder<Entities.Customer> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasColumnType("varchar(200)");

        builder.Property(c => c.SocialNumber).IsRequired()
            .HasColumnType("varchar(50)");

        builder.OwnsOne(c => c.Email, tf =>
        {
            tf.Property(c => c.Address)
                .IsRequired()
                .HasColumnName("Email")
                .HasColumnType($"varchar({Email.AddressMaxLength})");
        });
        
        builder.HasOne(c => c.Address)
            .WithOne(c => c.Customer);

        builder.ToTable("Customer");
    }
}