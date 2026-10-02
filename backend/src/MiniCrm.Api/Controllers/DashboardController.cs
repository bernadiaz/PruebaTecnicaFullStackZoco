using Microsoft.AspNetCore.Mvc;
using MiniCrm.Application.DTOs;
using MiniCrm.Application.Services;

namespace MiniCrm.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboard;

    public DashboardController(DashboardService dashboard)
    {
        _dashboard = dashboard;
    }

    [HttpGet("resumen")]
    public async Task<ActionResult<DashboardResumenDto>> GetResumen(CancellationToken cancellationToken)
    {
        return Ok(await _dashboard.GetResumenAsync(cancellationToken));
    }
}
