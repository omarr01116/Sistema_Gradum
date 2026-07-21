using SistemGradum.Application.DTOs.Proyecto;
using SistemGradum.Application.Exceptions;
using SistemGradum.Application.Interfaces;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Services;

public class ProyectoService : IProyectoService
{
    private readonly IProyectoRepository proyectoRepository;
    private readonly IClienteRepository clienteRepository;
    private readonly IAsesorRepository asesorRepository;
    private readonly IConfiguracionSistemaRepository configuracionRepository;

    public ProyectoService(
        IProyectoRepository proyectoRepository,
        IClienteRepository clienteRepository,
        IAsesorRepository asesorRepository,
        IConfiguracionSistemaRepository configuracionRepository)
    {
        this.proyectoRepository = proyectoRepository;
        this.clienteRepository = clienteRepository;
        this.asesorRepository = asesorRepository;
        this.configuracionRepository = configuracionRepository;
    }

    public async Task<List<ProyectoResponseDto>> GetAllAsync()
    {
        var proyectos = await this.proyectoRepository.GetAllAsync();
        return proyectos.Select(MapToResponse).ToList();
    }

    public async Task<ProyectoResponseDto?> GetByIdAsync(int id)
    {
        var proyecto = await this.proyectoRepository.GetByIdAsync(id);
        return proyecto is null ? null : MapToResponse(proyecto);
    }

    public async Task<ProyectoResponseDto> CreateAsync(CreateProyectoDto dto)
    {
        // RN-01: el cliente debe existir y estar activo
        var cliente = await this.clienteRepository.GetByIdAsync(dto.ClienteId);
        if (cliente is null || !cliente.Activo)
            throw new ReglaNegocioException("El cliente indicado no existe o está inactivo.");

        // El asesor debe existir y estar activo
        var asesor = await this.asesorRepository.GetByIdAsync(dto.AsesorId);
        if (asesor is null || !asesor.Activo)
            throw new ReglaNegocioException("El asesor indicado no existe o está inactivo.");

        // RN-03: validar carga laboral del asesor
        await ValidarDisponibilidadAsesorAsync(dto.AsesorId);

        var proyecto = new Proyecto
        {
            CodigoProyecto = await GenerarCodigoAsync(),
            ClienteId = dto.ClienteId,
            Universidad = dto.Universidad,
            Carrera = dto.Carrera,
            Programa = dto.Programa,
            TipoProyecto = dto.TipoProyecto,
            Tema = dto.Tema,
            AsesorId = dto.AsesorId,
            FechaInicio = dto.FechaInicio,
            FechaEntregaComprometida = dto.FechaEntregaComprometida,
            EstadoProyecto = "Activo"
        };

        await this.proyectoRepository.AddAsync(proyecto);
        await this.proyectoRepository.SaveChangesAsync();

        return MapToResponse(proyecto);
    }

    public async Task<bool> UpdateAsync(int id, UpdateProyectoDto dto)
    {
        var proyecto = await this.proyectoRepository.GetByIdAsync(id);
        if (proyecto is null)
            return false;

        // RF-006: reasignar asesor solo si el proyecto no está Finalizado
        if (proyecto.AsesorId != dto.AsesorId)
        {
            if (proyecto.EstadoProyecto == "Finalizado")
                throw new ReglaNegocioException("No se puede reasignar el asesor de un proyecto Finalizado.");

            var nuevoAsesor = await this.asesorRepository.GetByIdAsync(dto.AsesorId);
            if (nuevoAsesor is null || !nuevoAsesor.Activo)
                throw new ReglaNegocioException("El nuevo asesor indicado no existe o está inactivo.");

            // RN-03: validar carga del nuevo asesor
            await ValidarDisponibilidadAsesorAsync(dto.AsesorId);

            proyecto.AsesorId = dto.AsesorId;
        }

        proyecto.Universidad = dto.Universidad;
        proyecto.Carrera = dto.Carrera;
        proyecto.Programa = dto.Programa;
        proyecto.TipoProyecto = dto.TipoProyecto;
        proyecto.Tema = dto.Tema;
        proyecto.FechaEntregaComprometida = dto.FechaEntregaComprometida;

        await this.proyectoRepository.UpdateAsync(proyecto);
        await this.proyectoRepository.SaveChangesAsync();

        return true;
    }

    private async Task ValidarDisponibilidadAsesorAsync(int asesorId)
    {
        var config = await this.configuracionRepository.GetByClaveAsync("LIMITE_PROYECTOS_ASESOR");
        var limite = config is not null ? int.Parse(config.Valor) : 5;

        var cargaActual = await this.proyectoRepository.CountActivosByAsesorAsync(asesorId);

        if (cargaActual >= limite)
            throw new ReglaNegocioException(
                $"El asesor ya tiene {cargaActual} proyecto(s) activo(s), alcanzando el límite configurado ({limite}).");
    }

    private async Task<string> GenerarCodigoAsync()
    {
        var siguiente = await this.proyectoRepository.CountAsync() + 1;
        return $"PRY-{siguiente:D4}";
    }

    private static ProyectoResponseDto MapToResponse(Proyecto proyecto)
    {
        return new ProyectoResponseDto
        {
            Id = proyecto.Id,
            CodigoProyecto = proyecto.CodigoProyecto,
            ClienteId = proyecto.ClienteId,
            Universidad = proyecto.Universidad,
            Carrera = proyecto.Carrera,
            Programa = proyecto.Programa,
            TipoProyecto = proyecto.TipoProyecto,
            Tema = proyecto.Tema,
            AsesorId = proyecto.AsesorId,
            FechaInicio = proyecto.FechaInicio,
            FechaEntregaComprometida = proyecto.FechaEntregaComprometida,
            EstadoProyecto = proyecto.EstadoProyecto
        };
    }
}