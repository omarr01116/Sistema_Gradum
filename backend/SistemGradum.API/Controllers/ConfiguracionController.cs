using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemGradum.Application.DTOs.Configuracion;
using SistemGradum.Application.Interfaces;

namespace SistemGradum.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class ConfiguracionController : ControllerBase
{
    private readonly IConfiguracionSistemaRepository configuracionRepository;

    public ConfiguracionController(IConfiguracionSistemaRepository configuracionRepository)
    {
        this.configuracionRepository = configuracionRepository;
    }

    // GET /api/configuracion/{clave}
    [HttpGet("{clave}")]
    public async Task<IActionResult> GetByClave(string clave)
    {
        var config = await this.configuracionRepository.GetByClaveAsync(clave);
        return config is null ? NotFound() : Ok(new { config.Clave, config.Valor, config.Descripcion });
    }

    // PUT /api/configuracion/{clave}
    [HttpPut("{clave}")]
    public async Task<IActionResult> Update(string clave, UpdateConfiguracionDto dto)
    {
        var config = await this.configuracionRepository.GetByClaveAsync(clave);
        if (config is null)
            return NotFound(new { mensaje = $"No existe una configuración con clave '{clave}'." });

        if (string.IsNullOrWhiteSpace(dto.Valor))
            return BadRequest(new { mensaje = "El valor no puede estar vacío." });

        config.Valor = dto.Valor;

        await this.configuracionRepository.UpdateAsync(config);
        await this.configuracionRepository.SaveChangesAsync();

        return NoContent();
    }
}