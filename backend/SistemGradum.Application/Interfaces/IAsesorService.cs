using SistemGradum.Application.DTOs.Asesor;

namespace SistemGradum.Application.Interfaces;

public interface IAsesorService
{
    Task<List<AsesorResponseDto>> GetAllAsync();
    Task<AsesorResponseDto?> GetByIdAsync(int id);
    Task<AsesorResponseDto> CreateAsync(CreateAsesorDto dto);
    Task<bool> UpdateAsync(int id, UpdateAsesorDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ReactivarAsync(int id); 
}