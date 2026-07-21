using SistemGradum.Application.DTOs.Hito;

namespace SistemGradum.Application.Interfaces;

public interface IHitoService
{
    Task<List<HitoResponseDto>?> GetByProyectoIdAsync(int proyectoId, int? asesorIdFiltro);
    Task<List<HitoResponseDto>> CrearLoteAsync(int proyectoId, CreateHitosLoteDto dto);
    Task<(bool Success, string? Error)> UpdateAsync(int id, UpdateHitoDto dto);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}