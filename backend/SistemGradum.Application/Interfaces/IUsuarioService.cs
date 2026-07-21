using SistemGradum.Application.DTOs.Usuario;

namespace SistemGradum.Application.Interfaces;

public interface IUsuarioService
{
    Task<List<UsuarioResponseDto>> GetAllAsync();
    Task<UsuarioResponseDto?> GetByIdAsync(int id);
    Task<(UsuarioResponseDto? Usuario, string? Error)> CreateAsync(CreateUsuarioDto dto);
    Task<(bool Success, string? Error)> UpdateAsync(int id, UpdateUsuarioDto dto);
    Task<(bool Success, string? Error)> DesactivarAsync(int id);
}