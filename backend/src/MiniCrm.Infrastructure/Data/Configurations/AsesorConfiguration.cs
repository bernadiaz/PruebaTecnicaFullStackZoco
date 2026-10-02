using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniCrm.Domain.Entities;

namespace MiniCrm.Infrastructure.Data.Configurations;

public class AsesorConfiguration : IEntityTypeConfiguration<Asesor>
{
    public void Configure(EntityTypeBuilder<Asesor> builder)
    {
        builder.ToTable("Asesores");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Nombre).HasMaxLength(120).IsRequired();
    }
}
