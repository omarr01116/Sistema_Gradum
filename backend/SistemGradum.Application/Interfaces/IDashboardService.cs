using SistemGradum.Application.DTOs.Dashboard;

namespace SistemGradum.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardGeneralDto> GetGeneralAsync();
    Task<List<ProximaEntregaDto>> GetProximasEntregasAsync(int asesorId);
}