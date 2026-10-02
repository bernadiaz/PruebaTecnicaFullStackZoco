using Microsoft.EntityFrameworkCore;
using MiniCrm.Application.Abstractions;
using MiniCrm.Application.DTOs;
using MiniCrm.Domain.Enums;

namespace MiniCrm.Application.Services;

public class DashboardService
{
    private readonly IAppDbContext _db;

    public DashboardService(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardResumenDto> GetResumenAsync(CancellationToken cancellationToken = default)
    {
        var hoy = DateTime.Today;
        var total = await _db.Clientes.CountAsync(cancellationToken);
        var prospectos = await _db.Clientes.CountAsync(c => c.Estado == EstadoCliente.Prospecto, cancellationToken);
        var interesados = await _db.Clientes.CountAsync(c => c.Estado == EstadoCliente.Interesado, cancellationToken);
        var vencidos = await _db.Clientes.CountAsync(
            c => c.ProximoContacto != null && c.ProximoContacto < hoy,
            cancellationToken);

        return new DashboardResumenDto(total, prospectos, interesados, vencidos);
    }
}
