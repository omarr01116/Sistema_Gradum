using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Interfaces;

public interface IObservacionRepository
{
    Task<List<Observacion>> GetByProyectoIdAsync(int proyectoId);
    Task AddAsync(Observacion observacion);
    Task SaveChangesAsync();
}