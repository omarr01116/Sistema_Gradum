using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemGradum.Application.DTOs;
using SistemGradum.Application.Interfaces;

namespace SistemGradum.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Coordinador")] // RF-003: el Asesor no gestiona clientes
public class ClienteController : ControllerBase
{
    private readonly IClienteService clienteService;

    public ClienteController(IClienteService clienteService)
    {
        this.clienteService = clienteService;
    }

    // GET /api/cliente — RF-003 (Coordinador, Admin)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clientes = await this.clienteService.GetAllAsync();
        return Ok(clientes);
    }

    // GET /api/cliente/5 — RF-003 (Coordinador, Admin)
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cliente = await this.clienteService.GetByIdAsync(id);
        return cliente is null ? NotFound() : Ok(cliente);
    }

    // POST /api/cliente — RF-002: Actor único es Coordinador
    [HttpPost]
    [Authorize(Roles = "Coordinador")]
    public async Task<IActionResult> Create([FromBody] CreateClienteDto dto)
    {
        var (cliente, error) = await this.clienteService.CreateAsync(dto);

        if (cliente is null)
            return BadRequest(new { mensaje = error });

        return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
    }

    // PUT /api/cliente/5 — RF-003 (Coordinador, Admin)
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClienteDto dto)
    {
        var (success, error) = await this.clienteService.UpdateAsync(id, dto);

        if (!success)
        {
            // "Cliente no encontrado." es el único caso que debe dar 404;
            // cualquier otro mensaje (ej. validación de EstadoFinanciero) es 400.
            return error == "Cliente no encontrado."
                ? NotFound(new { mensaje = error })
                : BadRequest(new { mensaje = error });
        }

        return NoContent();
    }

    // DELETE /api/cliente/5 (desactivar) — RF-003 (Coordinador, Admin)
    [HttpDelete("{id:int}")]
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