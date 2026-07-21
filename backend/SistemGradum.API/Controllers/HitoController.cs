using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemGradum.Application.DTOs.Hito;
using SistemGradum.Application.Exceptions;
using SistemGradum.Application.Interfaces;

namespace SistemGradum.API.Controllers;

[ApiController]
[Route("api")]
[Authorize(Roles = "Administrador,Coordinador,Asesor")]
public class HitoController : ControllerBase
{
    private readonly IHitoService hitoService;

    public HitoController(IHitoService hitoService)
    {
        this.hitoService = hitoService;
    }

    // GET /api/proyecto/{proyectoId}/hitos
    [HttpGet("proyecto/{proyectoId:int}/hitos")]
    public async Task<IActionResult> GetByProyecto(int proyectoId)
    {
        var hitos = await this.hitoService.GetByProyectoIdAsync(proyectoId, this.ObtenerAsesorIdFiltro());
        return hitos is null ? NotFound() : Ok(hitos);
    }

    // POST /api/proyecto/{proyectoId}/hitos — RF-007
    [HttpPost("proyecto/{proyectoId:int}/hitos")]
    [Authorize(Roles = "Coordinador")]
    public async Task<IActionResult> Create(int proyectoId, CreateHitosLoteDto dto)
    {
        try
        {
            var creados = await this.hitoService.CrearLoteAsync(proyectoId, dto);
            return CreatedAtAction(nameof(GetByProyecto), new { proyectoId }, creados);
        }
        catch (ReglaNegocioException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // PUT /api/hito/{id}
    [HttpPut("hito/{id:int}")]
    [Authorize(Roles = "Coordinador")]
    public async Task<IActionResult> Update(int id, UpdateHitoDto dto)
    {
        var (success, error) = await this.hitoService.UpdateAsync(id, dto);

        if (!success)
        {
            return error == "Hito no encontrado."
                ? NotFound(new { mensaje = error })
                : BadRequest(new { mensaje = error });
        }

        return NoContent();
    }

    // DELETE /api/hito/{id}
    [HttpDelete("hito/{id:int}")]
    [Authorize(Roles = "Coordinador")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await this.hitoService.DeleteAsync(id);

        if (!success)
        {
            return error == "Hito no encontrado."
                ? NotFound(new { mensaje = error })
                : BadRequest(new { mensaje = error });
        }

        return NoContent();
    }

    // RN-08: mismo patrón que ProyectoController.
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