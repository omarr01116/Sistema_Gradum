using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemGradum.Application.Interfaces;

namespace SistemGradum.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Coordinador,Asesor")]
public class AlertaController : ControllerBase
{
    private readonly IAlertaService alertaService;

    public AlertaController(IAlertaService alertaService)
    {
        this.alertaService = alertaService;
    }

    // GET /api/alerta — alertas del usuario autenticado
    [HttpGet]
    public async Task<IActionResult> GetMisAlertas()
    {
        var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (usuarioIdClaim is null || !int.TryParse(usuarioIdClaim, out var usuarioId))
            return Unauthorized();

        var alertas = await this.alertaService.GetByUsuarioAsync(usuarioId);
        return Ok(alertas);
    }

    [HttpPatch("{id:int}/leida")]
    public async Task<IActionResult> MarcarLeida(int id)
    {
        var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (usuarioIdClaim is null || !int.TryParse(usuarioIdClaim, out var usuarioId))
            return Unauthorized();

        var (success, error) = await this.alertaService.MarcarLeidaAsync(id, usuarioId);
        return success ? NoContent() : NotFound(new { mensaje = error });
    }
}