using SistemGradum.Application.DTOs.Observacion;
using SistemGradum.Application.Interfaces;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Services;

public class ObservacionService : IObservacionService
{
    private readonly IObservacionRepository observacionRepository;
    private readonly IProyectoRepository proyectoRepository;

    public ObservacionService(
        IObservacionRepository observacionRepository, IProyectoRepository proyectoRepository)
    {
        this.observacionRepository = observacionRepository;
        this.proyectoRepository = proyectoRepository;
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
        return observaciones.Select(MapToResponse).ToList();
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

        return (MapToResponse(observacion), null);
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