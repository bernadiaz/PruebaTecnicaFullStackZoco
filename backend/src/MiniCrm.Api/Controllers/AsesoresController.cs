using Microsoft.AspNetCore.Mvc;
using MiniCrm.Application.DTOs;
using MiniCrm.Application.Services;

namespace MiniCrm.Api.Controllers;

[ApiController]
[Route("api/asesores")]
public class AsesoresController : ControllerBase
{
    private readonly AsesorService _asesores;

    public AsesoresController(AsesorService asesores)
    {
        _asesores = asesores;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AsesorDto>>> Get(CancellationToken cancellationToken)
    {
        return Ok(await _asesores.ListAsync(cancellationToken));
    }
}
