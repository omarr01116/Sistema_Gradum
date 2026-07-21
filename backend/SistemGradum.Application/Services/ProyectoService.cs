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

    // RN-06: diccionario de transiciones válidas (sección 8 del Word).
    private static readonly Dictionary<string, string[]> TransicionesValidas = new()
    {
        ["Activo"] = new[] { "Pausado", "Correcciones", "Sustentado" },
        ["Pausado"] = new[] { "Activo" },
        ["Correcciones"] = new[] { "Activo" },
        ["Sustentado"] = new[] { "Finalizado" },
        ["Finalizado"] = Array.Empty<string>()
    };

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

    public async Task<List<ProyectoResponseDto>> GetAllAsync(int? asesorIdFiltro)
    {
        var proyectos = await this.proyectoRepository.GetAllAsync();

        // RN-08: un Asesor solo ve sus propios proyectos.
        if (asesorIdFiltro.HasValue)
            proyectos = proyectos.Where(p => p.AsesorId == asesorIdFiltro.Value).ToList();

        return proyectos.Select(MapToResponse).ToList();
    }

    public async Task<ProyectoResponseDto?> GetByIdAsync(int id, int? asesorIdFiltro)
    {
        var proyecto = await this.proyectoRepository.GetByIdAsync(id);

        if (proyecto is null)
            return null;

        // RN-08: si es un Asesor y el proyecto no es suyo, se trata como si no existiera.
        if (asesorIdFiltro.HasValue && proyecto.AsesorId != asesorIdFiltro.Value)
            return null;

        return MapToResponse(proyecto);
    }

    public async Task<ProyectoResponseDto> CreateAsync(CreateProyectoDto dto)
    {
        var cliente = await this.clienteRepository.GetByIdAsync(dto.ClienteId);
        if (cliente is null || !cliente.Activo)
            throw new ReglaNegocioException("El cliente indicado no existe o está inactivo.");

        var asesor = await this.asesorRepository.GetByIdAsync(dto.AsesorId);
        if (asesor is null || !asesor.Activo)
            throw new ReglaNegocioException("El asesor indicado no existe o está inactivo.");

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

        if (proyecto.AsesorId != dto.AsesorId)
        {
            if (proyecto.EstadoProyecto == "Finalizado")
                throw new ReglaNegocioException("No se puede reasignar el asesor de un proyecto Finalizado.");

            var nuevoAsesor = await this.asesorRepository.GetByIdAsync(dto.AsesorId);
            if (nuevoAsesor is null || !nuevoAsesor.Activo)
                throw new ReglaNegocioException("El nuevo asesor indicado no existe o está inactivo.");

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

    public async Task<bool> CambiarEstadoAsync(int id, CambiarEstadoDto dto, int usuarioId)
    {
        var proyecto = await this.proyectoRepository.GetByIdAsync(id);
        if (proyecto is null)
            return false;

        var estadoActual = proyecto.EstadoProyecto;
        var nuevoEstado = dto.NuevoEstado;

        // RN-06: la transición debe estar en el diccionario de transiciones válidas.
        if (!TransicionesValidas.TryGetValue(estadoActual, out var permitidas) || !permitidas.Contains(nuevoEstado))
        {
            var opciones = permitidas is { Length: > 0 }
                ? string.Join(", ", permitidas)
                : "ninguna (es un estado terminal)";

            throw new ReglaNegocioException(
                $"No se puede cambiar de '{estadoActual}' a '{nuevoEstado}'. Transiciones permitidas desde '{estadoActual}': {opciones}.");
        }

        // RN-02: bloqueo por deuda al avanzar a Sustentado o Finalizado.
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
            EstadoProyecto = proyecto.EstadoProyecto,
            FechaUltimoCambio = proyecto.FechaUltimoCambio
        };
    }
}