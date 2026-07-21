using SistemGradum.Application.DTOs.Alerta;

namespace SistemGradum.Application.Interfaces;

public interface IAlertaService
{
    Task GenerarAlertasAsync(int usuarioId, string rol, int? asesorIdFiltro);
    Task<List<AlertaResponseDto>> GetByUsuarioAsync(int usuarioId);
    Task<(bool Success, string? Error)> MarcarLeidaAsync(int id, int usuarioId);
}