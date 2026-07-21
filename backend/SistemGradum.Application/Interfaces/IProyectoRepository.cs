using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Interfaces;

public interface IProyectoRepository
{
    Task<List<Proyecto>> GetAllAsync();
    Task<List<(Proyecto Proyecto, decimal PorcentajeAvance)>> GetAllWithProgressAsync(int? asesorIdFiltro = null);
    Task<Proyecto?> GetByIdAsync(int id);
    Task<int> CountAsync();
    Task<int> CountActivosByAsesorAsync(int asesorId);
    Task AddAsync(Proyecto proyecto);
    Task UpdateAsync(Proyecto proyecto);
    Task SaveChangesAsync();
}