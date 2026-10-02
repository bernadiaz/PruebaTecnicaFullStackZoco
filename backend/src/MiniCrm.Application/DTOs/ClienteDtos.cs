using MiniCrm.Domain.Enums;

namespace MiniCrm.Application.DTOs;

public record ClienteListItemDto(
    int Id,
    string Nombre,
    string Cuit,
    string Telefono,
    string? Email,
    EstadoCliente Estado,
    string EstadoNombre,
    int AsesorId,
    string AsesorNombre,
    DateTime? ProximoContacto,
    DateTime FechaActualizacion,
    bool SeguimientoVencido,
    bool Eliminado);

public record ClienteDetailDto(
    int Id,
    string Nombre,
    string Cuit,
    string Telefono,
    string? Email,
    EstadoCliente Estado,
    string EstadoNombre,
    int AsesorId,
    string AsesorNombre,
    DateTime? ProximoContacto,
    DateTime FechaCreacion,
    DateTime FechaActualizacion,
    bool SeguimientoVencido,
    bool Eliminado);

public record CrearClienteRequest(
    string Nombre,
    string Cuit,
    string Telefono,
    string? Email,
    EstadoCliente Estado,
    int AsesorId);

public record ActualizarClienteRequest(
    string Nombre,
    string Cuit,
    string Telefono,
    string? Email,
    EstadoCliente Estado,
    int AsesorId);
