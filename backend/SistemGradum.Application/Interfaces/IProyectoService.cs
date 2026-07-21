using SistemGradum.Application.DTOs.Proyecto;

namespace SistemGradum.Application.Interfaces;

public interface IProyectoService
{
    Task<List<ProyectoResponseDto>> GetAllAsync(int? asesorIdFiltro);
    Task<ProyectoResponseDto?> GetByIdAsync(int id, int? asesorIdFiltro);
    Task<ProyectoResponseDto> CreateAsync(CreateProyectoDto dto);
    Task<bool> UpdateAsync(int id, UpdateProyectoDto dto);
    Task<bool> CambiarEstadoAsync(int id, CambiarEstadoDto dto, int usuarioId);
}