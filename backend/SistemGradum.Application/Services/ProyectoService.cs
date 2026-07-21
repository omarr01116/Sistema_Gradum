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
    private readonly IHitoRepository hitoRepository;   // ← nuevo

    public ProyectoService(
        IProyectoRepository proyectoRepository,
        IClienteRepository clienteRepository,
        IAsesorRepository asesorRepository,
        IConfiguracionSistemaRepository configuracionRepository,
        IHitoRepository hitoRepository)
    {
        this.proyectoRepository = proyectoRepository;
        this.clienteRepository = clienteRepository;
        this.asesorRepository = asesorRepository;
        this.configuracionRepository = configuracionRepository;
        this.hitoRepository = hitoRepository;
    }

    public async Task<List<ProyectoResponseDto>> GetAllAsync(int? asesorIdFiltro)
    {
        var proyectos = await this.proyectoRepository.GetAllAsync();

        if (asesorIdFiltro.HasValue)
            proyectos = proyectos.Where(p => p.AsesorId == asesorIdFiltro.Value).ToList();

        var resultado = new List<ProyectoResponseDto>();
        foreach (var proyecto in proyectos)
            resultado.Add(await this.MapToResponseAsync(proyecto));

        return resultado;
    }

    public async Task<ProyectoResponseDto?> GetByIdAsync(int id, int? asesorIdFiltro)
    {
        var proyecto = await this.proyectoRepository.GetByIdAsync(id);
        if (proyecto is null)
            return null;

        if (asesorIdFiltro.HasValue && proyecto.AsesorId != asesorIdFiltro.Value)
            return null;

        return await this.MapToResponseAsync(proyecto);
    }

    public async Task<ProyectoResponseDto> CreateAsync(CreateProyectoDto dto)
    {
        var cliente = await this.clienteRepository.GetByIdAsync(dto.ClienteId);
        if (cliente is null || !cliente.Activo)
            throw new ReglaNegocioException("El cliente indicado no existe o está inactivo.");

        var asesor = await this.asesorRepository.GetByIdAsync(dto.AsesorId);
        if (asesor is null || !asesor.Activo)
            throw new ReglaNegocioException("El asesor indicado no existe o está inactivo.");

        await this.ValidarDisponibilidadAsesorAsync(dto.AsesorId);

        var proyecto = new Proyecto
        {
            CodigoProyecto = await this.GenerarCodigoAsync(),
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

        return await this.MapToResponseAsync(proyecto);
    }

    public async Task<bool> UpdateAsync(int id, UpdateProyectoDto dto)
    {
        var proyecto = await this.proyectoRepository.GetByIdAsync(id);
        if (proyecto is null)
            return false;

        // Chequeo movido aquí: bloquea CUALQUIER edición si el proyecto ya está Finalizado,
        // no solo cuando se intenta reasignar el asesor.
        if (proyecto.EstadoProyecto == "Finalizado")
            throw new ReglaNegocioException("No se puede modificar un proyecto en estado Finalizado.");

        if (proyecto.AsesorId != dto.AsesorId)
        {
            var nuevoAsesor = await this.asesorRepository.GetByIdAsync(dto.AsesorId);
            if (nuevoAsesor is null || !nuevoAsesor.Activo)
                throw new ReglaNegocioException("El nuevo asesor indicado no existe o está inactivo.");

            await this.ValidarDisponibilidadAsesorAsync(dto.AsesorId);

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

    public async Task<bool> CambiarEstadoAsync(int id, CambiarEstadoDto dto, int usuarioId)
    {
        var proyecto = await this.proyectoRepository.GetByIdAsync(id);
        if (proyecto is null)
            return false;

        var estadoActual = proyecto.EstadoProyecto;
        var nuevoEstado = dto.NuevoEstado;

        if (!TransicionesValidas.TryGetValue(estadoActual, out var permitidas) || !permitidas.Contains(nuevoEstado))
        {
            var opciones = permitidas is { Length: > 0 }
                ? string.Join(", ", permitidas)
                : "ninguna (es un estado terminal)";

            throw new ReglaNegocioException(
                $"No se puede cambiar de '{estadoActual}' a '{nuevoEstado}'. Transiciones permitidas desde '{estadoActual}': {opciones}.");
        }

        if (nuevoEstado is "Sustentado" or "Finalizado")
        {
            var cliente = await this.clienteRepository.GetByIdAsync(proyecto.ClienteId);
            if (cliente is not null && cliente.EstadoFinanciero == "ConDeuda")
                throw new ReglaNegocioException(
                    "No se puede avanzar el proyecto porque el cliente tiene estado financiero 'ConDeuda'.");
        }

        proyecto.EstadoProyecto = nuevoEstado;
        proyecto.UsuarioUltimoCambioId = usuarioId;
        proyecto.FechaUltimoCambio = DateTime.UtcNow;

        await this.proyectoRepository.UpdateAsync(proyecto);
        await this.proyectoRepository.SaveChangesAsync();

        return true;
    }

    private static readonly Dictionary<string, string[]> TransicionesValidas = new()
    {
        ["Activo"] = new[] { "Pausado", "Correcciones", "Sustentado" },
        ["Pausado"] = new[] { "Activo" },
        ["Correcciones"] = new[] { "Activo" },
        ["Sustentado"] = new[] { "Finalizado" },
        ["Finalizado"] = Array.Empty<string>()
    };

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

    // RF-008: el avance se calcula al vuelo, no se almacena.
    private async Task<ProyectoResponseDto> MapToResponseAsync(Proyecto proyecto)
    {
        var porcentajeAvance = await this.hitoRepository.SumaPesoAprobadoAsync(proyecto.Id);

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
            EstadoProyecto = proyecto.EstadoProyecto,
            FechaUltimoCambio = proyecto.FechaUltimoCambio,
            PorcentajeAvance = porcentajeAvance
        };
    }
}