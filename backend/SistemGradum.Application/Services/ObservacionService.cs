using SistemGradum.Application.DTOs.Observacion;
using SistemGradum.Application.Interfaces;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Services;

public class ObservacionService : IObservacionService
{
    private readonly IObservacionRepository observacionRepository;
    private readonly IProyectoRepository proyectoRepository;
    private readonly IUsuarioRepository usuarioRepository;

    public ObservacionService(
        IObservacionRepository observacionRepository,
        IProyectoRepository proyectoRepository,
        IUsuarioRepository usuarioRepository)
    {
        this.observacionRepository = observacionRepository;
        this.proyectoRepository = proyectoRepository;
        this.usuarioRepository = usuarioRepository;
    }

    public async Task<List<ObservacionResponseDto>?> GetByProyectoIdAsync(int proyectoId, int? asesorIdFiltro)
    {
        var proyecto = await this.proyectoRepository.GetByIdAsync(proyectoId);
        if (proyecto is null)
            return null;

        // RN-08: el Asesor solo ve observaciones de sus propios proyectos.
        if (asesorIdFiltro.HasValue && proyecto.AsesorId != asesorIdFiltro.Value)
            return null;

        var observaciones = await this.observacionRepository.GetByProyectoIdAsync(proyectoId);
        
        var usuarioIds = observaciones.Select(o => o.UsuarioId).Distinct().ToList();
        var usuarios = new Dictionary<int, string>();
        foreach (var uid in usuarioIds)
        {
            var u = await this.usuarioRepository.GetByIdAsync(uid);
            if (u is not null)
            {
                usuarios[uid] = u.NombreUsuario;
            }
        }

        var result = new List<ObservacionResponseDto>();
        foreach (var o in observaciones)
        {
            var dto = MapToResponse(o);
            dto.NombreUsuario = usuarios.TryGetValue(o.UsuarioId, out var name) ? name : $"Usuario #{o.UsuarioId}";
            result.Add(dto);
        }

        return result;
    }

    public async Task<(ObservacionResponseDto? Observacion, string? Error)> CrearAsync(
        int proyectoId, CreateObservacionDto dto, int usuarioId, int? asesorIdFiltro)
    {
        var proyecto = await this.proyectoRepository.GetByIdAsync(proyectoId);
        if (proyecto is null)
            return (null, "Proyecto no encontrado.");

        // RN-08: el Asesor solo registra observaciones en sus propios proyectos.
        if (asesorIdFiltro.HasValue && proyecto.AsesorId != asesorIdFiltro.Value)
            return (null, "Proyecto no encontrado.");

        if (string.IsNullOrWhiteSpace(dto.Detalle))
            return (null, "El detalle de la observación es obligatorio.");

        var observacion = new Observacion
        {
            ProyectoId = proyectoId,
            UsuarioId = usuarioId,
            Detalle = dto.Detalle,
            FechaHora = DateTime.UtcNow
        };

        await this.observacionRepository.AddAsync(observacion);
        await this.observacionRepository.SaveChangesAsync();

        var user = await this.usuarioRepository.GetByIdAsync(usuarioId);
        var nombreUsuario = user?.NombreUsuario ?? $"Usuario #{usuarioId}";

        var resDto = MapToResponse(observacion);
        resDto.NombreUsuario = nombreUsuario;

        return (resDto, null);
    }

    private static ObservacionResponseDto MapToResponse(Observacion o) => new()
    {
        Id = o.Id,
        ProyectoId = o.ProyectoId,
        UsuarioId = o.UsuarioId,
        FechaHora = o.FechaHora,
        Detalle = o.Detalle
    };
}