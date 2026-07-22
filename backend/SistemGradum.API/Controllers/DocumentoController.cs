using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SistemGradum.Application.Interfaces;
using SistemGradum.Application.DTOs.Documento;

namespace SistemGradum.API.Controllers;

[ApiController]
[Route("api")]
[Authorize(Roles = "Administrador,Coordinador,Asesor")]
public class DocumentoController : ControllerBase
{
    private readonly IDocumentoService documentoService;

    public DocumentoController(IDocumentoService documentoService)
    {
        this.documentoService = documentoService;
    }

    // GET /api/proyecto/{proyectoId}/documentos
    [HttpGet("proyecto/{proyectoId:int}/documentos")]
    public async Task<IActionResult> GetByProyecto(int proyectoId)
    {
        var documentos = await this.documentoService.GetByProyectoIdAsync(proyectoId, this.ObtenerAsesorIdFiltro());
        return documentos is null ? NotFound() : Ok(documentos);
    }

    // POST /api/proyecto/{proyectoId}/documentos — RF-013
    [HttpPost("proyecto/{proyectoId:int}/documentos")]
    [Authorize(Roles = "Administrador,Asesor,Coordinador")]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> Subir(
        int proyectoId, [FromForm] SubirDocumentoDto dto)
    {
        var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (usuarioIdClaim is null || !int.TryParse(usuarioIdClaim, out var usuarioId))
            return Unauthorized();

        // Usamos dto.Categoria y dto.Archivo
        var (success, error, resultado) = await this.documentoService.SubirAsync(
            proyectoId, dto.Categoria, dto.Archivo, usuarioId, this.ObtenerAsesorIdFiltro());

        if (!success)
        {
            return error == "Proyecto no encontrado."
                ? NotFound(new { mensaje = error })
                : BadRequest(new { mensaje = error });
        }

        return CreatedAtAction(nameof(Versiones), new { id = resultado!.Id }, resultado);
    }

    // GET /api/documento/{id}/versiones — RF-014
    [HttpGet("documento/{id:int}/versiones")]
    public async Task<IActionResult> Versiones(int id)
    {
        var (success, error, resultado) = await this.documentoService.GetVersionesAsync(
            id, this.ObtenerAsesorIdFiltro());

        return success ? Ok(resultado) : NotFound(new { mensaje = error });
    }

    // GET /api/documento/{id}/version/{numero}/descargar — RF-014
    [HttpGet("documento/{id:int}/version/{numero:int}/descargar")]
    public async Task<IActionResult> Descargar(int id, int numero)
    {
        var (success, error, stream, nombreDescarga) =
            await this.documentoService.ObtenerParaDescargaAsync(id, numero, this.ObtenerAsesorIdFiltro());

        if (!success)
            return NotFound(new { mensaje = error });

        return File(stream!, "application/octet-stream", nombreDescarga);
    }

    // RN-08: mismo patrón que ProyectoController y HitoController.
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