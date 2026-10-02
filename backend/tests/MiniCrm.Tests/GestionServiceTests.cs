using FluentAssertions;
using MiniCrm.Application.DTOs;
using Xunit;
using MiniCrm.Application.Services;
using MiniCrm.Domain.Entities;
using MiniCrm.Domain.Enums;

namespace MiniCrm.Tests;

public class GestionServiceTests
{
    [Fact]
    public async Task RegisterAsync_UpdatesClienteEstadoAndKeepsPreviousGestiones()
    {
        using var fixture = new SqliteTestContext();
        var cliente = new Cliente
        {
            Nombre = "TechSur SRL",
            Cuit = "30-71234567-8",
            CuitNormalizado = "30712345678",
            Telefono = "3814550202",
            Estado = EstadoCliente.Prospecto,
            AsesorId = fixture.Asesor.Id,
            FechaCreacion = DateTime.UtcNow.AddDays(-2),
            FechaActualizacion = DateTime.UtcNow.AddDays(-2)
        };
        fixture.Db.Clientes.Add(cliente);
        fixture.Db.Gestiones.Add(new Gestion
        {
            Cliente = cliente,
            TipoContacto = TipoContacto.Llamada,
            Comentario = "Primera llamada",
            EstadoResultante = EstadoCliente.Prospecto,
            FechaGestion = DateTime.UtcNow.AddDays(-1)
        });
        await fixture.Db.SaveChangesAsync();

        var service = new GestionService(fixture.Db);
        await service.RegisterAsync(cliente.Id, new CrearGestionRequest(
            TipoContacto.Reunion,
            "Reunión comercial. Quedaron interesados.",
            EstadoCliente.Interesado,
            DateTime.Today.AddDays(3)));

        var updated = await fixture.Db.Clientes.FindAsync(cliente.Id);
        updated!.Estado.Should().Be(EstadoCliente.Interesado);
        fixture.Db.Gestiones.Count(g => g.ClienteId == cliente.Id).Should().Be(2);
    }

    [Fact]
    public async Task RegisterAsync_WhenProximoContactoIsProvided_UpdatesCliente()
    {
        using var fixture = new SqliteTestContext();
        var cliente = new Cliente
        {
            Nombre = "Farmacia San Martín",
            Cuit = "27-30111222-3",
            CuitNormalizado = "27301112223",
            Telefono = "3814550303",
            Estado = EstadoCliente.Contactado,
            AsesorId = fixture.Asesor.Id,
            ProximoContacto = DateTime.Today.AddDays(-1),
            FechaCreacion = DateTime.UtcNow.AddDays(-4),
            FechaActualizacion = DateTime.UtcNow.AddDays(-4)
        };
        fixture.Db.Clientes.Add(cliente);
        await fixture.Db.SaveChangesAsync();

        var proximo = DateTime.Today.AddDays(7);
        var service = new GestionService(fixture.Db);
        await service.RegisterAsync(cliente.Id, new CrearGestionRequest(
            TipoContacto.WhatsApp,
            "Reprogramamos el contacto para la semana próxima.",
            EstadoCliente.Interesado,
            proximo));

        var updated = await fixture.Db.Clientes.FindAsync(cliente.Id);
        updated!.ProximoContacto.Should().Be(proximo);
        updated.Estado.Should().Be(EstadoCliente.Interesado);
    }
}
