using Microsoft.EntityFrameworkCore;
using MiniCrm.Application.Abstractions;
using MiniCrm.Application.Common;
using MiniCrm.Application.DTOs;
using MiniCrm.Application.Exceptions;
using MiniCrm.Domain.Entities;
using MiniCrm.Domain.Enums;

namespace MiniCrm.Application.Services;

public class GestionService
{
    private readonly IAppDbContext _db;

    public GestionService(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<GestionDto>> ListByClienteAsync(int clienteId, CancellationToken cancellationToken = default)
    {
        await EnsureClienteExistsAsync(clienteId, cancellationToken);

        var gestiones = await _db.Gestiones
            .AsNoTracking()
            .Where(g => g.ClienteId == clienteId)
            .OrderByDescending(g => g.FechaGestion)
            .ThenByDescending(g => g.Id)
            .ToListAsync(cancellationToken);

        return gestiones.Select(Map).ToList();
    }

    public async Task<GestionDto> RegisterAsync(int clienteId, CrearGestionRequest request, CancellationToken cancellationToken = default)
    {
        var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.Id == clienteId, cancellationToken);
        if (cliente is null)
        {
            throw new NotFoundException($"No se encontró el cliente {clienteId}.");
        }

        ValidateGestion(request);

        var now = DateTime.UtcNow;
        var gestion = new Gestion
        {
            ClienteId = clienteId,
            TipoContacto = request.TipoContacto,
            Comentario = request.Comentario.Trim(),
            EstadoResultante = request.EstadoResultante,
            FechaGestion = now,
            ProximoContacto = request.ProximoContacto
        };

        _db.Gestiones.Add(gestion);
        cliente.Estado = request.EstadoResultante;
        if (request.ProximoContacto.HasValue)
        {
            cliente.ProximoContacto = request.ProximoContacto;
        }

        cliente.FechaActualizacion = now;
        await _db.SaveChangesAsync(cancellationToken);
        return Map(gestion);
    }

    private async Task EnsureClienteExistsAsync(int clienteId, CancellationToken cancellationToken)
    {
        var exists = await _db.Clientes.AnyAsync(c => c.Id == clienteId, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException($"No se encontró el cliente {clienteId}.");
        }
    }

    private static void ValidateGestion(CrearGestionRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (!Enum.IsDefined(request.TipoContacto))
        {
            errors["tipoContacto"] = ["El tipo de contacto no es un valor permitido."];
        }

        if (string.IsNullOrWhiteSpace(request.Comentario))
        {
            errors["comentario"] = ["El comentario es obligatorio."];
        }

        if (!Enum.IsDefined(request.EstadoResultante))
        {
            errors["estadoResultante"] = ["El estado resultante no es un valor permitido."];
        }

        if (errors.Count > 0)
        {
            throw new BusinessValidationException("Los datos de la gestión no son válidos.", errors);
        }
    }

    private static GestionDto Map(Gestion gestion) => new(
        gestion.Id,
        gestion.TipoContacto,
        EnumLabels.For(gestion.TipoContacto),
        gestion.Comentario,
        gestion.EstadoResultante,
        EnumLabels.For(gestion.EstadoResultante),
        gestion.FechaGestion,
        gestion.ProximoContacto);
}
