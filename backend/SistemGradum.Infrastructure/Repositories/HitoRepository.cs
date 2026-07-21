using Microsoft.EntityFrameworkCore;
using SistemGradum.Application.Interfaces;
using SistemGradum.Domain.Entities;
using SistemGradum.Infrastructure.Data;

namespace SistemGradum.Infrastructure.Repositories;

public class HitoRepository : IHitoRepository
{
    private readonly SistemGradumDbContext context;

    public HitoRepository(SistemGradumDbContext context)
    {
        this.context = context;
    }

    public async Task<List<Hito>> GetByProyectoIdAsync(int proyectoId)
    {
        return await this.context.Hitos
            .AsNoTracking()
            .Where(h => h.ProyectoId == proyectoId)
            .OrderBy(h => h.Orden)
            .ToListAsync();
    }

    public async Task<Hito?> GetByIdAsync(int id)
    {
        return await this.context.Hitos
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task AddRangeAsync(IEnumerable<Hito> hitos)
    {
        await this.context.Hitos.AddRangeAsync(hitos);
    }

    public async Task UpdateAsync(Hito hito)
    {
        this.context.Hitos.Update(hito);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Hito hito)
    {
        this.context.Hitos.Remove(hito);
        await Task.CompletedTask;
    }

    public async Task<bool> ExisteHitoAprobadoEnProyectoAsync(int proyectoId)
    {
        return await this.context.Hitos
            .AnyAsync(h => h.ProyectoId == proyectoId && h.EstadoHito == "Aprobado");
    }

    public async Task SaveChangesAsync()
    {
        await this.context.SaveChangesAsync();
    }

    public async Task<decimal> SumaPesoAprobadoAsync(int proyectoId)
    {
        return await this.context.Hitos
            .Where(h => h.ProyectoId == proyectoId && h.EstadoHito == "Aprobado")
            .SumAsync(h => h.PesoPorcentual);
    }
}