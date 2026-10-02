using Microsoft.EntityFrameworkCore;
using MiniCrm.Application.Abstractions;
using MiniCrm.Application.Common;
using MiniCrm.Application.DTOs;
using MiniCrm.Application.Exceptions;
using MiniCrm.Domain.Entities;
using MiniCrm.Domain.Enums;

namespace MiniCrm.Application.Services;

public class ClienteService
{
    public const int PageSize = 5;

    private readonly IAppDbContext _db;

    public ClienteService(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<ClienteListItemDto>> ListAsync(
        string? search,
        EstadoCliente? estado,
        int? asesorId = null,
        int page = 1,
        bool soloEliminados = false,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Clientes
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Include(c => c.Asesor)
            .Where(c => c.Eliminado == soloEliminados);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            var normalized = CuitNormalizer.Normalize(term);
            query = query.Where(c =>
                c.Nombre.ToLower().Contains(term) ||
                c.Cuit.ToLower().Contains(term) ||
                c.Telefono.ToLower().Contains(term) ||
                (!string.IsNullOrEmpty(normalized) && c.CuitNormalizado.Contains(normalized)));
        }

        if (estado.HasValue)
        {
            query = query.Where(c => c.Estado == estado.Value);
        }

        if (asesorId.HasValue)
        {
            query = query.Where(c => c.AsesorId == asesorId.Value);
        }

        if (page < 1)
        {
            page = 1;
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling(totalCount / (double)PageSize);

        if (totalPages > 0 && page > totalPages)
        {
            page = totalPages;
        }

        var clientes = await query
            .OrderBy(c => c.ProximoContacto == null)
            .ThenBy(c => c.ProximoContacto)
            .ThenBy(c => c.Nombre)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<ClienteListItemDto>(
            clientes.Select(MapListItem).ToList(),
            page,
            PageSize,
            totalCount,
            totalPages);
    }

    public async Task<ClienteDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cliente = await _db.Clientes
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Include(c => c.Asesor)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (cliente is null)
        {
            throw new NotFoundException($"No se encontró el cliente {id}.");
        }

        return MapDetail(cliente);
    }

    public async Task<ClienteDetailDto> CreateAsync(CrearClienteRequest request, CancellationToken cancellationToken = default)
    {
        ValidateCliente(request.Nombre, request.Cuit, request.Telefono, request.Email, request.Estado);

        var asesor = await GetAsesorAsync(request.AsesorId, cancellationToken);
        var cuitNormalizado = CuitNormalizer.Normalize(request.Cuit);
        await EnsureCuitIsUniqueAsync(cuitNormalizado, excludeId: null, cancellationToken);

        var now = ArgentinaTime.Now;
        var cliente = new Cliente
        {
            Nombre = request.Nombre.Trim(),
            Cuit = request.Cuit.Trim(),
            CuitNormalizado = cuitNormalizado,
            Telefono = request.Telefono.Trim(),
            Email = NormalizeEmail(request.Email),
            Estado = request.Estado,
            AsesorId = asesor.Id,
            Asesor = asesor,
            FechaCreacion = now,
            FechaActualizacion = now
        };

        _db.Clientes.Add(cliente);
        await SaveSafelyAsync(cancellationToken);
        return MapDetail(cliente);
    }

    public async Task<ClienteDetailDto> UpdateAsync(int id, ActualizarClienteRequest request, CancellationToken cancellationToken = default)
    {
        ValidateCliente(request.Nombre, request.Cuit, request.Telefono, request.Email, request.Estado);

        var cliente = await _db.Clientes
            .Include(c => c.Asesor)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (cliente is null)
        {
            throw new NotFoundException($"No se encontró el cliente {id}.");
        }

        var asesor = await GetAsesorAsync(request.AsesorId, cancellationToken);
        var cuitNormalizado = CuitNormalizer.Normalize(request.Cuit);
        await EnsureCuitIsUniqueAsync(cuitNormalizado, excludeId: id, cancellationToken);

        cliente.Nombre = request.Nombre.Trim();
        cliente.Cuit = request.Cuit.Trim();
        cliente.CuitNormalizado = cuitNormalizado;
        cliente.Telefono = request.Telefono.Trim();
        cliente.Email = NormalizeEmail(request.Email);
        cliente.Estado = request.Estado;
        cliente.AsesorId = asesor.Id;
        cliente.Asesor = asesor;
        cliente.FechaActualizacion = ArgentinaTime.Now;

        await SaveSafelyAsync(cancellationToken);
        return MapDetail(cliente);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var cliente = await _db.Clientes
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (cliente is null || cliente.Eliminado)
        {
            throw new NotFoundException($"No se encontró el cliente {id}.");
        }

        var now = ArgentinaTime.Now;
        cliente.Eliminado = true;
        cliente.FechaEliminacion = now;
        cliente.FechaActualizacion = now;
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<ClienteDetailDto> RestoreAsync(int id, CancellationToken cancellationToken = default)
    {
        var cliente = await _db.Clientes
            .IgnoreQueryFilters()
            .Include(c => c.Asesor)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (cliente is null)
        {
            throw new NotFoundException($"No se encontró el cliente {id}.");
        }

        if (!cliente.Eliminado)
        {
            throw new BusinessValidationException("El cliente no está dado de baja.");
        }

        var now = ArgentinaTime.Now;
        cliente.Eliminado = false;
        cliente.FechaEliminacion = null;
        cliente.FechaActualizacion = now;
        await _db.SaveChangesAsync(cancellationToken);
        return MapDetail(cliente);
    }

    private static void ValidateCliente(
        string nombre,
        string cuit,
        string telefono,
        string? email,
        EstadoCliente estado)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(nombre))
        {
            errors["nombre"] = ["El nombre es obligatorio."];
        }

        var cuitNormalizado = CuitNormalizer.Normalize(cuit);
        if (string.IsNullOrWhiteSpace(cuit) || cuitNormalizado.Length == 0)
        {
            errors["cuit"] = ["El CUIT es obligatorio."];
        }
        else if (cuitNormalizado.Length != 11)
        {
            errors["cuit"] = ["El CUIT debe tener 11 dígitos."];
        }

        if (string.IsNullOrWhiteSpace(telefono))
        {
            errors["telefono"] = ["El teléfono es obligatorio."];
        }

        if (!EmailValidator.IsValid(email))
        {
            errors["email"] = ["El correo electrónico no tiene un formato válido."];
        }

        if (!Enum.IsDefined(estado))
        {
            errors["estado"] = ["El estado no es un valor permitido."];
        }

        if (errors.Count > 0)
        {
            throw new BusinessValidationException("Los datos del cliente no son válidos.", errors);
        }
    }

    private async Task<Asesor> GetAsesorAsync(int asesorId, CancellationToken cancellationToken)
    {
        var asesor = await _db.Asesores.FirstOrDefaultAsync(a => a.Id == asesorId, cancellationToken);
        if (asesor is null)
        {
            throw new BusinessValidationException(
                "El asesor indicado no existe.",
                new Dictionary<string, string[]> { ["asesorId"] = ["El asesor no existe."] });
        }

        return asesor;
    }

    private async Task EnsureCuitIsUniqueAsync(string cuitNormalizado, int? excludeId, CancellationToken cancellationToken)
    {
        var exists = await _db.Clientes
            .IgnoreQueryFilters()
            .AnyAsync(
                c => c.CuitNormalizado == cuitNormalizado
                     && (!excludeId.HasValue || c.Id != excludeId.Value),
                cancellationToken);

        if (exists)
        {
            throw new ConflictException("Ya existe un cliente con el mismo CUIT.");
        }
    }

    private async Task SaveSafelyAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            throw new ConflictException("Ya existe un cliente con el mismo CUIT.");
        }
    }

    private static string? NormalizeEmail(string? email) =>
        string.IsNullOrWhiteSpace(email) ? null : email.Trim();

    private static ClienteListItemDto MapListItem(Cliente cliente) => new(
        cliente.Id,
        cliente.Nombre,
        cliente.Cuit,
        cliente.Telefono,
        cliente.Email,
        cliente.Estado,
        EnumLabels.For(cliente.Estado),
        cliente.AsesorId,
        cliente.Asesor.Nombre,
        cliente.ProximoContacto,
        cliente.FechaActualizacion,
        SeguimientoRules.EstaVencido(cliente.ProximoContacto),
        cliente.Eliminado);

    private static ClienteDetailDto MapDetail(Cliente cliente) => new(
        cliente.Id,
        cliente.Nombre,
        cliente.Cuit,
        cliente.Telefono,
        cliente.Email,
        cliente.Estado,
        EnumLabels.For(cliente.Estado),
        cliente.AsesorId,
        cliente.Asesor.Nombre,
        cliente.ProximoContacto,
        cliente.FechaCreacion,
        cliente.FechaActualizacion,
        SeguimientoRules.EstaVencido(cliente.ProximoContacto),
        cliente.Eliminado);
}
