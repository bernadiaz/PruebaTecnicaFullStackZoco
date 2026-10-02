using FluentAssertions;
using MiniCrm.Application.DTOs;
using Xunit;
using MiniCrm.Application.Exceptions;
using MiniCrm.Application.Services;
using MiniCrm.Domain.Enums;

namespace MiniCrm.Tests;

public class ClienteServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenCuitAlreadyExists_ThrowsConflict()
    {
        using var fixture = new SqliteTestContext();
        var service = new ClienteService(fixture.Db);
        var request = new CrearClienteRequest(
            "Panadería El Trigal",
            "20-28333444-5",
            "3814550101",
            "trigal@correo.com",
            EstadoCliente.Prospecto,
            fixture.Asesor.Id);

        await service.CreateAsync(request);

        var duplicate = request with { Nombre = "Otro comercio", Cuit = "20283334445" };
        var act = async () => await service.CreateAsync(duplicate);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*CUIT*");
    }

    [Fact]
    public async Task CreateAsync_WhenEmailIsInvalid_ThrowsValidation()
    {
        using var fixture = new SqliteTestContext();
        var service = new ClienteService(fixture.Db);
        var request = new CrearClienteRequest(
            "Café del Puerto",
            "20-33445566-7",
            "3814550505",
            "correo-invalido",
            EstadoCliente.Prospecto,
            fixture.Asesor.Id);

        var act = async () => await service.CreateAsync(request);

        await act.Should().ThrowAsync<BusinessValidationException>();
    }
}
