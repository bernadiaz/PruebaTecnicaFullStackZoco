using Microsoft.AspNetCore.Mvc;
using MiniCrm.Application.DTOs;
using MiniCrm.Application.Services;
using MiniCrm.Domain.Enums;

namespace MiniCrm.Api.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController : ControllerBase
{
    private readonly ClienteService _clientes;
    private readonly GestionService _gestiones;

    public ClientesController(ClienteService clientes, GestionService gestiones)
    {
        _clientes = clientes;
        _gestiones = gestiones;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ClienteListItemDto>>> Get(
        [FromQuery] string? search,
        [FromQuery] EstadoCliente? estado,
        CancellationToken cancellationToken)
    {
        return Ok(await _clientes.ListAsync(search, estado, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteDetailDto>> GetById(int id, CancellationToken cancellationToken)
    {
        return Ok(await _clientes.GetByIdAsync(id, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<ClienteDetailDto>> Post(
        [FromBody] CrearClienteRequest request,
        CancellationToken cancellationToken)
    {
        var created = await _clientes.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ClienteDetailDto>> Put(
        int id,
        [FromBody] ActualizarClienteRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await _clientes.UpdateAsync(id, request, cancellationToken));
    }

    [HttpGet("{id:int}/gestiones")]
    public async Task<ActionResult<IReadOnlyList<GestionDto>>> GetGestiones(int id, CancellationToken cancellationToken)
    {
        return Ok(await _gestiones.ListByClienteAsync(id, cancellationToken));
    }

    [HttpPost("{id:int}/gestiones")]
    public async Task<ActionResult<GestionDto>> PostGestion(
        int id,
        [FromBody] CrearGestionRequest request,
        CancellationToken cancellationToken)
    {
        var created = await _gestiones.RegisterAsync(id, request, cancellationToken);
        return CreatedAtAction(nameof(GetGestiones), new { id }, created);
    }
}
