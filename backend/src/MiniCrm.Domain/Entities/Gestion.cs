using MiniCrm.Domain.Enums;

namespace MiniCrm.Domain.Entities;

public class Gestion
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public TipoContacto TipoContacto { get; set; }
    public string Comentario { get; set; } = string.Empty;
    public EstadoCliente EstadoResultante { get; set; }
    public DateTime FechaGestion { get; set; }
    public DateTime? ProximoContacto { get; set; }
}
