using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemGradum.Application.DTOs;
using SistemGradum.Application.Interfaces;

namespace SistemGradum.API.Controllers;

[ApiController]
[Route("api/[controller]")]
// CORRECCIÓN: Se agrega Asesor para que pueda ejecutar los GET y resolver los nombres en el tablero.
[Authorize(Roles = "Administrador,Coordinador,Asesor")] 
public class ClienteController : ControllerBase
{
    private readonly IClienteService clienteService;

    public ClienteController(IClienteService clienteService)
    {
        this.clienteService = clienteService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clientes = await this.clienteService.GetAllAsync();
        return Ok(clientes);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cliente = await this.clienteService.GetByIdAsync(id);
        return cliente is null ? NotFound() : Ok(cliente);
    }

    [HttpPost]
    [Authorize(Roles = "Coordinador")]
    public async Task<IActionResult> Create([FromBody] CreateClienteDto dto)
    {
        var (cliente, error) = await this.clienteService.CreateAsync(dto);

        if (cliente is null)
            return BadRequest(new { mensaje = error });

        return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
    }

    [HttpPut("{id:int}")]
    // CORRECCIÓN: Restringido estrictamente a Administrador y Coordinador
    [Authorize(Roles = "Administrador,Coordinador")] 
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClienteDto dto)
    {
        var (success, error) = await this.clienteService.UpdateAsync(id, dto);

        if (!success)
        {
            return error == "Cliente no encontrado."
                ? NotFound(new { mensaje = error })
                : BadRequest(new { mensaje = error });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    // CORRECCIÓN: Restringido estrictamente a Administrador y Coordinador
    [Authorize(Roles = "Administrador,Coordinador")] 
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await this.clienteService.DeleteAsync(id);
        if (!success)
            return NotFound(new { mensaje = error });

        return NoContent();
    }

    [HttpPatch("{id:int}/reactivar")]
    [Authorize(Roles = "Administrador,Coordinador")]
    public async Task<IActionResult> Reactivar(int id)
    {
        var reactivado = await this.clienteService.ReactivarAsync(id);

        return reactivado
            ? NoContent()
            : NotFound();
    }
}