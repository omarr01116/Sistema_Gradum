using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
    private readonly IDocumentoService documentoService;   // ← faltaba

    public HitoController(IHitoService hitoService, IDocumentoService documentoService)
    {
        this.hitoService = hitoService;
        this.documentoService = documentoService;           // ← faltaba
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
    [Authorize(Roles = "Administrador,Coordinador")]
    public async Task<IActionResult> Create(int proyectoId, CreateHitosLoteDto dto)
    {
        try
        {
            var creados = await this.hitoService.CrearLoteAsync(proyectoId, dto);
            return CreatedAtAction(nameof(this.GetByProyecto), new { proyectoId }, creados);
        }
        catch (ReglaNegocioException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // PUT /api/hito/{id}
    [HttpPut("hito/{id:int}")]
    [Authorize(Roles = "Administrador,Coordinador")]
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
    [Authorize(Roles = "Administrador,Coordinador")]
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

    // PATCH /api/hito/{id}/completar — RF-009
    [HttpPatch("hito/{id:int}/completar")]
    [Authorize(Roles = "Asesor")]
    public async Task<IActionResult> Completar(int id, [FromForm] CompletarHitoDto dto)
    {
        var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (usuarioIdClaim is null || !int.TryParse(usuarioIdClaim, out var usuarioId))
            return Unauthorized();

        int? documentoId = null;

        // Usamos dto.Evidencia en lugar de la variable suelta
        if (dto.Evidencia is not null)
        {
            var hitoActual = await this.hitoService.ObtenerProyectoIdAsync(id);
            if (hitoActual is null)
                return NotFound(new { mensaje = "Hito no encontrado." });

            var (subOk, subError, subResultado) = await this.documentoService.SubirAsync(
                hitoActual.Value, $"Evidencia-Hito-{id}", dto.Evidencia, usuarioId, this.ObtenerAsesorIdFiltro());

            if (!subOk)
                return BadRequest(new { mensaje = subError });

            documentoId = subResultado!.Id;
        }

        var (success, error) = await this.hitoService.CompletarAsync(id, documentoId, usuarioId, this.ObtenerAsesorIdFiltro());
        
        return success
            ? NoContent()
            : (error == "Hito no encontrado." ? NotFound(new { mensaje = error }) : BadRequest(new { mensaje = error }));
    }

    // PATCH /api/hito/{id}/aprobar — RF-010
    [HttpPatch("hito/{id:int}/aprobar")]
    [Authorize(Roles = "Administrador,Coordinador")]
    public async Task<IActionResult> Aprobar(int id)
    {
        var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (usuarioIdClaim is null || !int.TryParse(usuarioIdClaim, out var usuarioId))
            return Unauthorized();

        var (success, error) = await this.hitoService.AprobarAsync(id, usuarioId);

        if (!success)
        {
            return error == "Hito no encontrado."
                ? NotFound(new { mensaje = error })
                : BadRequest(new { mensaje = error });
        }

        return NoContent();
    }

    // PATCH /api/hito/{id}/rechazar — RN-05
    [HttpPatch("hito/{id:int}/rechazar")]
    [Authorize(Roles = "Administrador,Coordinador")]
    public async Task<IActionResult> Rechazar(int id, RechazarHitoDto dto)
    {
        var (success, error) = await this.hitoService.RechazarAsync(id, dto);

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
        if (asesorIdClaim is null || !int.TryParse(asesorIdClaim, out var asesorId))
        {
            throw new UnauthorizedAccessException("El claim AsesorId es inválido o no existe en la sesión actual.");
        }

        return asesorId;
    }
}