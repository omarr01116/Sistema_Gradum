using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemGradum.Application.DTOs.Proyecto;
using SistemGradum.Application.Exceptions;
using SistemGradum.Application.Interfaces;

namespace SistemGradum.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Coordinador,Asesor")]
public class ProyectoController : ControllerBase
{
    private readonly IProyectoService proyectoService;

    public ProyectoController(IProyectoService proyectoService)
    {
        this.proyectoService = proyectoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var proyectos = await this.proyectoService.GetAllAsync(ObtenerAsesorIdFiltro());
        return Ok(proyectos);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var proyecto = await this.proyectoService.GetByIdAsync(id, ObtenerAsesorIdFiltro());
        return proyecto is null ? NotFound() : Ok(proyecto);
    }

    [HttpPost]
    [Authorize(Roles = "Coordinador")]
    public async Task<IActionResult> Create(CreateProyectoDto dto)
    {
        try
        {
            var creado = await this.proyectoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
        }
        catch (ReglaNegocioException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Coordinador")]
    public async Task<IActionResult> Update(int id, UpdateProyectoDto dto)
    {
        try
        {
            var actualizado = await this.proyectoService.UpdateAsync(id, dto);
            return actualizado ? NoContent() : NotFound();
        }
        catch (ReglaNegocioException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPatch("{id:int}/estado")]
    [Authorize(Roles = "Coordinador")]
    public async Task<IActionResult> CambiarEstado(int id, CambiarEstadoDto dto)
    {
        var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (usuarioIdClaim is null || !int.TryParse(usuarioIdClaim, out var usuarioId))
            return Unauthorized();

        try
        {
            var actualizado = await this.proyectoService.CambiarEstadoAsync(id, dto, usuarioId);
            return actualizado ? NoContent() : NotFound();
        }
        catch (ReglaNegocioException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // RN-08: si el usuario logueado es Asesor, devuelve su IdAsesor (del claim del JWT)
    // para que el Service filtre. Si es Coordinador/Administrador, devuelve null (ve todo).
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