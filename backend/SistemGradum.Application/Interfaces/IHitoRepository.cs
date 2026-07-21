using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Interfaces;

public interface IHitoRepository
{
    Task<List<Hito>> GetByProyectoIdAsync(int proyectoId);
    Task<Hito?> GetByIdAsync(int id);
    Task AddRangeAsync(IEnumerable<Hito> hitos);
    Task UpdateAsync(Hito hito);
    Task DeleteAsync(Hito hito);
    Task<bool> ExisteHitoAprobadoEnProyectoAsync(int proyectoId);
    Task SaveChangesAsync();
}