using SistemGradum.Application.DTOs;

namespace SistemGradum.Application.Interfaces;

public interface IClienteService
{
    Task<List<ClienteResponseDto>> GetAllAsync();
    Task<ClienteResponseDto?> GetByIdAsync(int id);
    Task<ClienteResponseDto> CreateAsync(CreateClienteDto dto);
    Task<(bool Success, string? Error)> UpdateAsync(int id, UpdateClienteDto dto);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}