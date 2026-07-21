using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemGradum.Application.DTOs.Asesor;
using SistemGradum.Application.Interfaces;

namespace SistemGradum.API.Controllers;

[ApiController]
[Route("api/[controller]")]
// CORRECCIÓN: Se agrega Asesor para que pueda ver la lista en el frontend.
[Authorize(Roles = "Administrador,Coordinador,Asesor")]
public class AsesorController : ControllerBase
{
    private readonly IAsesorService asesorService;

    public AsesorController(IAsesorService asesorService)
    {
        this.asesorService = asesorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var asesores = await this.asesorService.GetAllAsync();
        return Ok(asesores);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var asesor = await this.asesorService.GetByIdAsync(id);

        return asesor is null
            ? NotFound()
            : Ok(asesor);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create(CreateAsesorDto dto)
    {
        var creado = await this.asesorService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = creado.Id },
            creado);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(int id, UpdateAsesorDto dto)
    {
        var actualizado = await this.asesorService.UpdateAsync(id, dto);

        return actualizado
            ? NoContent()
            : NotFound();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id)
    {
        var eliminado = await this.asesorService.DeleteAsync(id);

        return eliminado
            ? NoContent()
            : NotFound();
    }

    [HttpPatch("{id:int}/reactivar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Reactivar(int id)
    {
        var reactivado = await this.asesorService.ReactivarAsync(id);

        return reactivado
            ? NoContent()
            : NotFound();
    }
}