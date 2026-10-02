using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniCrm.Domain.Entities;

namespace MiniCrm.Infrastructure.Data.Configurations;

public class GestionConfiguration : IEntityTypeConfiguration<Gestion>
{
    public void Configure(EntityTypeBuilder<Gestion> builder)
    {
        builder.ToTable("Gestiones");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TipoContacto).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(x => x.Comentario).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.EstadoResultante).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.HasIndex(x => new { x.ClienteId, x.FechaGestion });
    }
}
