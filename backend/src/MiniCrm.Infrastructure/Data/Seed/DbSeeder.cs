using Microsoft.EntityFrameworkCore;
using MiniCrm.Application.Common;
using MiniCrm.Domain.Entities;
using MiniCrm.Domain.Enums;

namespace MiniCrm.Infrastructure.Data.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Asesores.AnyAsync(cancellationToken))
        {
            return;
        }

        var asesores = new[]
        {
            new Asesor { Nombre = "Laura Fernández" },
            new Asesor { Nombre = "Martín Álvarez" },
            new Asesor { Nombre = "Sofía Ruiz" },
            new Asesor { Nombre = "Diego Pérez" }
        };

        db.Asesores.AddRange(asesores);
        await db.SaveChangesAsync(cancellationToken);

        var hoy = DateTime.Today;
        var creado = DateTime.UtcNow.AddDays(-20);

        var clientes = new[]
        {
            CrearCliente("Panadería El Trigal", "20-28333444-5", "3814550101", "trigal@correo.com",
                EstadoCliente.Prospecto, asesores[0].Id, hoy.AddDays(-5), creado),
            CrearCliente("TechSur SRL", "30-71234567-8", "3814550202", "contacto@techsur.com",
                EstadoCliente.Contactado, asesores[1].Id, hoy.AddDays(4), creado.AddDays(2)),
            CrearCliente("Farmacia San Martín", "27-30111222-3", "3814550303", "farmacia@sanmartin.com",
                EstadoCliente.Interesado, asesores[2].Id, hoy.AddDays(2), creado.AddDays(4)),
            CrearCliente("Transportes Andinos", "30-69874512-1", "3814550404", null,
                EstadoCliente.NoInteresado, asesores[3].Id, null, creado.AddDays(5)),
            CrearCliente("Café del Puerto", "20-33445566-7", "3814550505", "hola@cafedelpuerto.com",
                EstadoCliente.Cliente, asesores[0].Id, hoy.AddDays(15), creado.AddDays(6)),
            CrearCliente("Distribuidora Norte", "33-55667788-9", "3814550606", "ventas@distnorte.com",
                EstadoCliente.Prospecto, asesores[1].Id, hoy.AddDays(7), creado.AddDays(8)),
            CrearCliente("Taller Mecánico Roca", "23-40998877-4", "3814550707", "roca@taller.com",
                EstadoCliente.Contactado, asesores[2].Id, hoy.AddDays(-2), creado.AddDays(9))
        };

        db.Clientes.AddRange(clientes);
        await db.SaveChangesAsync(cancellationToken);

        db.Gestiones.AddRange(
            new Gestion
            {
                ClienteId = clientes[0].Id,
                TipoContacto = TipoContacto.Llamada,
                Comentario = "Primera llamada. Pidieron información de planes y volvieron a consultar precio.",
                EstadoResultante = EstadoCliente.Prospecto,
                FechaGestion = DateTime.UtcNow.AddDays(-8),
                ProximoContacto = hoy.AddDays(-5)
            },
            new Gestion
            {
                ClienteId = clientes[1].Id,
                TipoContacto = TipoContacto.Correo,
                Comentario = "Enviamos propuesta comercial y confirmaron recepción.",
                EstadoResultante = EstadoCliente.Contactado,
                FechaGestion = DateTime.UtcNow.AddDays(-3),
                ProximoContacto = hoy.AddDays(4)
            },
            new Gestion
            {
                ClienteId = clientes[2].Id,
                TipoContacto = TipoContacto.Reunion,
                Comentario = "Reunión presencial. Mostraron interés en el plan intermedio.",
                EstadoResultante = EstadoCliente.Interesado,
                FechaGestion = DateTime.UtcNow.AddDays(-1),
                ProximoContacto = hoy.AddDays(2)
            },
            new Gestion
            {
                ClienteId = clientes[3].Id,
                TipoContacto = TipoContacto.WhatsApp,
                Comentario = "Indicaron que por ahora no van a avanzar.",
                EstadoResultante = EstadoCliente.NoInteresado,
                FechaGestion = DateTime.UtcNow.AddDays(-6)
            },
            new Gestion
            {
                ClienteId = clientes[4].Id,
                TipoContacto = TipoContacto.Llamada,
                Comentario = "Cerraron la operatoria. Cliente activo.",
                EstadoResultante = EstadoCliente.Cliente,
                FechaGestion = DateTime.UtcNow.AddDays(-10),
                ProximoContacto = hoy.AddDays(15)
            },
            new Gestion
            {
                ClienteId = clientes[6].Id,
                TipoContacto = TipoContacto.Otro,
                Comentario = "Visita al taller. Quedó pendiente recontacto y no respondieron.",
                EstadoResultante = EstadoCliente.Contactado,
                FechaGestion = DateTime.UtcNow.AddDays(-4),
                ProximoContacto = hoy.AddDays(-2)
            },
            new Gestion
            {
                ClienteId = clientes[1].Id,
                TipoContacto = TipoContacto.WhatsApp,
                Comentario = "Recordatorio de la propuesta enviada. Coordinar nueva llamada.",
                EstadoResultante = EstadoCliente.Contactado,
                FechaGestion = DateTime.UtcNow.AddDays(-1),
                ProximoContacto = hoy.AddDays(4)
            });

        await db.SaveChangesAsync(cancellationToken);
    }

    private static Cliente CrearCliente(
        string nombre,
        string cuit,
        string telefono,
        string? email,
        EstadoCliente estado,
        int asesorId,
        DateTime? proximoContacto,
        DateTime fechaCreacion)
    {
        return new Cliente
        {
            Nombre = nombre,
            Cuit = cuit,
            CuitNormalizado = CuitNormalizer.Normalize(cuit),
            Telefono = telefono,
            Email = email,
            Estado = estado,
            AsesorId = asesorId,
            ProximoContacto = proximoContacto,
            FechaCreacion = fechaCreacion,
            FechaActualizacion = fechaCreacion
        };
    }
}
