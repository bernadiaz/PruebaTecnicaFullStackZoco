using Microsoft.EntityFrameworkCore;
using MiniCrm.Domain.Entities;

namespace MiniCrm.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<Asesor> Asesores { get; }
    DbSet<Cliente> Clientes { get; }
    DbSet<Gestion> Gestiones { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
