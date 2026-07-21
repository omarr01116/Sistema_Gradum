using Microsoft.EntityFrameworkCore;
using SistemGradum.Application.Interfaces;
using SistemGradum.Domain.Entities;
using SistemGradum.Infrastructure.Data;

namespace SistemGradum.Infrastructure.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly SistemGradumDbContext context;

    public ClienteRepository(SistemGradumDbContext context)
    {
        this.context = context;
    }

    public async Task<List<Cliente>> GetAllAsync()
    {
        // RF-003: "manteniendo su historial de proyectos intacto" → no se borra físicamente,
        // el desactivado simplemente se filtra de los listados normales.
        return await this.context.Clientes
            .Where(c => c.Activo)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        // Sin AsNoTracking: Update (Paso 6) necesita la entidad trackeada.
        return await this.context.Clientes
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<int> CountAsync()
    {
        // Cuenta TODOS los clientes (activos e inactivos) para que el código
        // nunca se repita, aunque un cliente haya sido desactivado.
        return await this.context.Clientes.CountAsync();
    }

    public async Task AddAsync(Cliente cliente)
    {
        await this.context.Clientes.AddAsync(cliente);
    }

    public Task UpdateAsync(Cliente cliente)
    {
        this.context.Clientes.Update(cliente);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await this.context.SaveChangesAsync();
    }
}