namespace MiniCrm.Domain.Entities;

public class Asesor
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
}
