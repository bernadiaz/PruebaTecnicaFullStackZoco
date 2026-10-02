using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniCrm.Domain.Entities;

namespace MiniCrm.Infrastructure.Data.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Nombre).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Cuit).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CuitNormalizado).HasMaxLength(11).IsRequired();
        builder.HasIndex(x => x.CuitNormalizado).IsUnique();
        builder.Property(x => x.Telefono).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.Property(x => x.Estado).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(x => x.Eliminado).HasDefaultValue(false);
        builder.HasIndex(x => x.Eliminado);
        builder.HasQueryFilter(x => !x.Eliminado);
        builder.HasOne(x => x.Asesor)
            .WithMany(x => x.Clientes)
            .HasForeignKey(x => x.AsesorId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Gestiones)
            .WithOne(x => x.Cliente)
            .HasForeignKey(x => x.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
