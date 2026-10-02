using Microsoft.EntityFrameworkCore;
using MiniCrm.Application.Abstractions;
using MiniCrm.Domain.Entities;

namespace MiniCrm.Infrastructure.Data;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Asesor> Asesores => Set<Asesor>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Gestion> Gestiones => Set<Gestion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
