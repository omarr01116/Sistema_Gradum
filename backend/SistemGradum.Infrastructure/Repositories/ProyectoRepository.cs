using Microsoft.EntityFrameworkCore;
using SistemGradum.Application.Interfaces;
using SistemGradum.Domain.Entities;
using SistemGradum.Infrastructure.Data;

namespace SistemGradum.Infrastructure.Repositories;

public class ProyectoRepository : IProyectoRepository
{
    private readonly SistemGradumDbContext context;

    public ProyectoRepository(SistemGradumDbContext context)
    {
        this.context = context;
    }

    public async Task<List<Proyecto>> GetAllAsync()
    {
        return await this.context.Proyectos
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<(Proyecto Proyecto, decimal PorcentajeAvance)>> GetAllWithProgressAsync(int? asesorIdFiltro = null)
    {
        var query = this.context.Proyectos.AsNoTracking().AsQueryable();
        if (asesorIdFiltro.HasValue)
        {
            query = query.Where(p => p.AsesorId == asesorIdFiltro.Value);
        }

        var result = await query
            .Select(p => new
            {
                Proyecto = p,
                PorcentajeAvance = p.Hitos
                    .Where(h => h.EstadoHito == "Aprobado")
                    .Sum(h => (decimal?)h.PesoPorcentual) ?? 0m
            })
            .ToListAsync();

        return result.Select(r => (r.Proyecto, r.PorcentajeAvance)).ToList();
    }

    public async Task<Proyecto?> GetByIdAsync(int id)
    {
        return await this.context.Proyectos
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<int> CountAsync()
    {
        return await this.context.Proyectos.CountAsync();
    }

    public async Task<int> CountActivosByAsesorAsync(int asesorId)
    {
        return await this.context.Proyectos
            .CountAsync(p => p.AsesorId == asesorId && p.EstadoProyecto != "Finalizado");
    }

    public async Task AddAsync(Proyecto proyecto)
    {
        await this.context.Proyectos.AddAsync(proyecto);
    }

    public async Task UpdateAsync(Proyecto proyecto)
    {
        this.context.Proyectos.Update(proyecto);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await this.context.SaveChangesAsync();
    }
}