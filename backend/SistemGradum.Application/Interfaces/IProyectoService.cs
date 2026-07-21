using SistemGradum.Application.DTOs.Proyecto;

namespace SistemGradum.Application.Interfaces;

public interface IProyectoService
{
    Task<List<ProyectoResponseDto>> GetAllAsync();
    Task<ProyectoResponseDto?> GetByIdAsync(int id);
    Task<ProyectoResponseDto> CreateAsync(CreateProyectoDto dto);
    Task<bool> UpdateAsync(int id, UpdateProyectoDto dto);
}