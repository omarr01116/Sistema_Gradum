using Microsoft.AspNetCore.Mvc;
using SistemGradum.Application.DTOs;
using SistemGradum.Application.Interfaces;

namespace SistemGradum.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;

    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("login")]
    public IActionResult IniciarSesion([FromBody] LoginRequestDto dto)
    {
        var resultado = this.authService.IniciarSesion(dto);
        if (resultado is null)
            return Unauthorized(new { mensaje = "Credenciales inválidas" });

        return Ok(resultado);
    }
}