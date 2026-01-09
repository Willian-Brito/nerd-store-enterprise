using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSE.Security.Jwt.Core.Model;

namespace NSE.Security.Jwt.Store.EntityFrameworkCore;

public class KeyMaterialMap : IEntityTypeConfiguration<KeyMaterial>
{
    public void Configure(EntityTypeBuilder<KeyMaterial> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Parameters)
            .HasMaxLength(8000)
            .IsRequired();
    }
}