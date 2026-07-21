using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemGradum.Application.Interfaces;

namespace SistemGradum.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Coordinador,Asesor")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        this.dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (User.IsInRole("Asesor"))
        {
            var asesorIdClaim = User.FindFirst("AsesorId")?.Value;
            if (asesorIdClaim is null || !int.TryParse(asesorIdClaim, out var asesorId))
                return Unauthorized();

            var proximasEntregas = await this.dashboardService.GetProximasEntregasAsync(asesorId);
            return Ok(new { proximasEntregas });
        }

        var general = await this.dashboardService.GetGeneralAsync();
        return Ok(general);
    }
}