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
        var proyectos = await this.proyectoService.GetAllAsync();
        return Ok(proyectos);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var proyecto = await this.proyectoService.GetByIdAsync(id);
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
}