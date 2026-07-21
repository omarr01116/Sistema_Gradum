using SistemGradum.Application.DTOs.Observacion;

namespace SistemGradum.Application.Interfaces;

public interface IObservacionService
{
    Task<List<ObservacionResponseDto>?> GetByProyectoIdAsync(int proyectoId, int? asesorIdFiltro);
    Task<(ObservacionResponseDto? Observacion, string? Error)> CrearAsync(
        int proyectoId, CreateObservacionDto dto, int usuarioId, int? asesorIdFiltro);
}