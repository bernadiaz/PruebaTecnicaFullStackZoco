using MiniCrm.Domain.Enums;

namespace MiniCrm.Application.DTOs;

public record GestionDto(
    int Id,
    TipoContacto TipoContacto,
    string TipoContactoNombre,
    string Comentario,
    EstadoCliente EstadoResultante,
    string EstadoResultanteNombre,
    DateTime FechaGestion,
    DateTime? ProximoContacto);

public record CrearGestionRequest(
    TipoContacto TipoContacto,
    string Comentario,
    EstadoCliente EstadoResultante,
    DateTime? ProximoContacto);
