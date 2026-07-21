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
        if (config is null)
        {
            // Retornar un valor por defecto si no existe en la BD
            return Ok(new { Clave = clave, Valor = "5", Descripcion = "Límite de proyectos activos por asesor" });
        }
        return Ok(new { config.Clave, config.Valor, config.Descripcion });
    }

    // PUT /api/configuracion/{clave}
    [HttpPut("{clave}")]
    public async Task<IActionResult> Update(string clave, UpdateConfiguracionDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Valor))
            return BadRequest(new { mensaje = "El valor no puede estar vacío." });

        var config = await this.configuracionRepository.GetByClaveAsync(clave);
        if (config is null)
        {
            // En lugar de NotFound, lo creamos
            config = new SistemGradum.Domain.Entities.ConfiguracionSistema
            {
                Clave = clave,
                Valor = dto.Valor,
                Descripcion = "Límite de proyectos activos por asesor"
            };
            await this.configuracionRepository.AddAsync(config);
        }
        else
        {
            config.Valor = dto.Valor;
            await this.configuracionRepository.UpdateAsync(config);
        }

        await this.configuracionRepository.SaveChangesAsync();

        return NoContent();
    }
}