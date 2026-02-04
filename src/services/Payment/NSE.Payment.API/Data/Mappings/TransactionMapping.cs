using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models = NSE.Payment.API.Models;

namespace NSE.Payment.API.Data.Mappings;

public class TransactionMapping : IEntityTypeConfiguration<Models.Transaction>
{
    public void Configure(EntityTypeBuilder<Models.Transaction> builder)
    {
        builder.HasKey(c => c.Id);

        // 1 : N => Payment : Transaction
        builder.HasOne(c => c.Payment)
            .WithMany(c => c.Transactions);

        builder.ToTable("Transactions");
    }
}