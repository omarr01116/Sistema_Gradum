using Microsoft.AspNetCore.Mvc;
using SistemGradum.Application.DTOs;
using SistemGradum.Application.Interfaces;

namespace SistemGradum.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly IClienteService clienteService;

    public ClienteController(IClienteService clienteService)
    {
        this.clienteService = clienteService;
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        return Ok(this.clienteService.ObtenerTodos());
    }

    [HttpGet("{id}")]
    public IActionResult ObtenerPorId(int id)
    {
        var cliente = this.clienteService.ObtenerPorId(id);
        if (cliente is null) return NotFound();
        return Ok(cliente);
    }

    [HttpPost]
    public IActionResult Crear([FromBody] CreateClienteDto dto)
    {
        var cliente = this.clienteService.Crear(dto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = cliente.Id }, cliente);
    }
}