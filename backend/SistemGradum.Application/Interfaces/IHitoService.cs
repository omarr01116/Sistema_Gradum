using SistemGradum.Application.DTOs.Hito;

namespace SistemGradum.Application.Interfaces;

public interface IHitoService
{
    Task<List<HitoResponseDto>?> GetByProyectoIdAsync(int proyectoId, int? asesorIdFiltro);
    Task<List<HitoResponseDto>> CrearLoteAsync(int proyectoId, CreateHitosLoteDto dto);
    Task<(bool Success, string? Error)> UpdateAsync(int id, UpdateHitoDto dto);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
    Task<(bool Success, string? Error)> CompletarAsync(int id, CompletarHitoDto dto, int usuarioId, int? asesorIdFiltro);
    Task<(bool Success, string? Error)> AprobarAsync(int id, int usuarioId);
    Task<(bool Success, string? Error)> RechazarAsync(int id, RechazarHitoDto dto);
}