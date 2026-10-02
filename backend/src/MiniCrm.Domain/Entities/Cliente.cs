using MiniCrm.Domain.Enums;

namespace MiniCrm.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Cuit { get; set; } = string.Empty;
    public string CuitNormalizado { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string? Email { get; set; }
    public EstadoCliente Estado { get; set; }
    public int AsesorId { get; set; }
    public Asesor Asesor { get; set; } = null!;
    public DateTime? ProximoContacto { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }
    public ICollection<Gestion> Gestiones { get; set; } = new List<Gestion>();
}
