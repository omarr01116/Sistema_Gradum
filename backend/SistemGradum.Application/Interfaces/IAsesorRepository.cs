using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Interfaces;

public interface IAsesorRepository
{
    Task<List<Asesor>> GetAllAsync();
    Task<Asesor?> GetByIdAsync(int id);
    Task AddAsync(Asesor asesor);
    Task UpdateAsync(Asesor asesor);
    Task SaveChangesAsync();
}