using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Interfaces;

public interface IClienteRepository
{
    Task<List<Cliente>> GetAllAsync();
    Task<Cliente?> GetByIdAsync(int id);
    Task<int> CountAsync(); // usado para generar codigo_cliente autogenerado (RF-002)
    Task AddAsync(Cliente cliente);
    Task UpdateAsync(Cliente cliente);
    Task SaveChangesAsync();
}