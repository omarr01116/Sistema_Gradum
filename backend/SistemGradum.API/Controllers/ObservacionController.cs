using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemGradum.Application.DTOs.Observacion;
using SistemGradum.Application.Interfaces;

namespace SistemGradum.API.Controllers;

[ApiController]
[Route("api")]
[Authorize(Roles = "Administrador,Coordinador,Asesor")]
public class ObservacionController : ControllerBase
{
    private readonly IObservacionService observacionService;

    public ObservacionController(IObservacionService observacionService)
    {
        this.observacionService = observacionService;
    }

    // GET /api/proyecto/{proyectoId}/observaciones — RF-012
    [HttpGet("proyecto/{proyectoId:int}/observaciones")]
    public async Task<IActionResult> GetByProyecto(int proyectoId)
    {
        var observaciones = await this.observacionService.GetByProyectoIdAsync(
            proyectoId, this.ObtenerAsesorIdFiltro());

        return observaciones is null ? NotFound() : Ok(observaciones);
    }

    // POST /api/proyecto/{proyectoId}/observaciones — RF-012
    [HttpPost("proyecto/{proyectoId:int}/observaciones")]
    [Authorize(Roles = "Asesor,Coordinador")]
    public async Task<IActionResult> Crear(int proyectoId, CreateObservacionDto dto)
    {
        var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (usuarioIdClaim is null || !int.TryParse(usuarioIdClaim, out var usuarioId))
            return Unauthorized();

        var (observacion, error) = await this.observacionService.CrearAsync(
            proyectoId, dto, usuarioId, this.ObtenerAsesorIdFiltro());

        if (observacion is null)
        {
            return error == "Proyecto no encontrado."
                ? NotFound(new { mensaje = error })
                : BadRequest(new { mensaje = error });
        }

        return CreatedAtAction(nameof(GetByProyecto), new { proyectoId }, observacion);
    }

    private int? ObtenerAsesorIdFiltro()
    {
        if (!User.IsInRole("Asesor"))
            return null;

        var asesorIdClaim = User.FindFirst("AsesorId")?.Value;
        return asesorIdClaim is not null && int.TryParse(asesorIdClaim, out var asesorId)
            ? asesorId
            : null;
    }
}