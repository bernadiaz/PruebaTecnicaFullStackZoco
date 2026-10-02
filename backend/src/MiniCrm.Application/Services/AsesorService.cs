using Microsoft.EntityFrameworkCore;
using MiniCrm.Application.Abstractions;
using MiniCrm.Application.DTOs;

namespace MiniCrm.Application.Services;

public class AsesorService
{
    private readonly IAppDbContext _db;

    public AsesorService(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<AsesorDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Asesores
            .AsNoTracking()
            .OrderBy(a => a.Nombre)
            .Select(a => new AsesorDto(a.Id, a.Nombre))
            .ToListAsync(cancellationToken);
    }
}
