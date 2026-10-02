using FluentAssertions;
using MiniCrm.Application.Common;
using Xunit;
using MiniCrm.Application.Services;
using MiniCrm.Domain.Entities;
using MiniCrm.Domain.Enums;

namespace MiniCrm.Tests;

public class DashboardAndRulesTests
{
    [Fact]
    public void EstaVencido_WhenProximoContactoIsBeforeToday_ReturnsTrue()
    {
        var hoy = new DateTime(2026, 10, 2);
        SeguimientoRules.EstaVencido(hoy.AddDays(-1), hoy).Should().BeTrue();
        SeguimientoRules.EstaVencido(hoy, hoy).Should().BeFalse();
        SeguimientoRules.EstaVencido(null, hoy).Should().BeFalse();
    }

    [Fact]
    public async Task GetResumenAsync_CountsOverdueFollowUps()
    {
        using var fixture = new SqliteTestContext();
        fixture.Db.Clientes.AddRange(
            new Cliente
            {
                Nombre = "Vencido Uno",
                Cuit = "20-11111111-1",
                CuitNormalizado = "20111111111",
                Telefono = "111",
                Estado = EstadoCliente.Prospecto,
                AsesorId = fixture.Asesor.Id,
                ProximoContacto = DateTime.Today.AddDays(-3),
                FechaCreacion = DateTime.UtcNow,
                FechaActualizacion = DateTime.UtcNow
            },
            new Cliente
            {
                Nombre = "Interesado",
                Cuit = "20-22222222-2",
                CuitNormalizado = "20222222222",
                Telefono = "222",
                Estado = EstadoCliente.Interesado,
                AsesorId = fixture.Asesor.Id,
                ProximoContacto = DateTime.Today.AddDays(2),
                FechaCreacion = DateTime.UtcNow,
                FechaActualizacion = DateTime.UtcNow
            });
        await fixture.Db.SaveChangesAsync();

        var resumen = await new DashboardService(fixture.Db).GetResumenAsync();

        resumen.TotalClientes.Should().Be(2);
        resumen.Prospectos.Should().Be(1);
        resumen.Interesados.Should().Be(1);
        resumen.SeguimientosVencidos.Should().Be(1);
    }
}
