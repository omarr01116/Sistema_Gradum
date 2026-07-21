using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemGradum.Application.DTOs;
using SistemGradum.Application.Interfaces;

namespace SistemGradum.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClienteController : ControllerBase
{
    private readonly IClienteService clienteService;

    public ClienteController(IClienteService clienteService)
    {
        this.clienteService = clienteService;
    }

    [Authorize(Roles = "Administrador,Coordinador")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClienteDto dto)
    {
        var resultado = await this.clienteService.Crear(dto); // <- reemplaza por el nombre real
        return Ok(resultado);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var resultado = await this.clienteService.ObtenerTodos(); // <- reemplaza por el nombre real
        return Ok(resultado);
    }
}