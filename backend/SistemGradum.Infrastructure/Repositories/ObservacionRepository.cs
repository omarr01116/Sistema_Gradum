using Microsoft.EntityFrameworkCore;
using SistemGradum.Application.Interfaces;
using SistemGradum.Domain.Entities;
using SistemGradum.Infrastructure.Data;

namespace SistemGradum.Infrastructure.Repositories;

public class ObservacionRepository : IObservacionRepository
{
    private readonly SistemGradumDbContext context;

    public ObservacionRepository(SistemGradumDbContext context)
    {
        this.context = context;
    }

    public async Task<List<Observacion>> GetByProyectoIdAsync(int proyectoId)
    {
        return await this.context.Observaciones
            .AsNoTracking()
            .Where(o => o.ProyectoId == proyectoId)
            .OrderByDescending(o => o.FechaHora)
            .ToListAsync();
    }

    public async Task AddAsync(Observacion observacion)
    {
        await this.context.Observaciones.AddAsync(observacion);
    }

    public async Task SaveChangesAsync()
    {
        await this.context.SaveChangesAsync();
    }
}