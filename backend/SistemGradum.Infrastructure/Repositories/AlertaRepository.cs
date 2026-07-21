using Microsoft.EntityFrameworkCore;
using SistemGradum.Application.Interfaces;
using SistemGradum.Domain.Entities;
using SistemGradum.Infrastructure.Data;

namespace SistemGradum.Infrastructure.Repositories;

public class AlertaRepository : IAlertaRepository
{
    private readonly SistemGradumDbContext context;

    public AlertaRepository(SistemGradumDbContext context)
    {
        this.context = context;
    }

    // RF-022: hitos con fecha_compromiso <= hoy + 3 días (y que aún no estén aprobados).
    public async Task<List<Hito>> GetHitosProximosAVencerAsync(int? asesorIdFiltro)
    {
        var limite = DateTime.UtcNow.Date.AddDays(3);

        var query = this.context.Hitos
            .Include(h => h.Proyecto)
            .Where(h => h.FechaCompromiso <= limite && h.EstadoHito != "Aprobado");

        if (asesorIdFiltro.HasValue)
            query = query.Where(h => h.Proyecto!.AsesorId == asesorIdFiltro.Value);

        return await query.AsNoTracking().ToListAsync();
    }

    // RF-022: proyectos en estado "Correcciones".
    public async Task<List<Proyecto>> GetProyectosEnCorreccionesAsync(int? asesorIdFiltro)
    {
        var query = this.context.Proyectos.Where(p => p.EstadoProyecto == "Correcciones");

        if (asesorIdFiltro.HasValue)
            query = query.Where(p => p.AsesorId == asesorIdFiltro.Value);

        return await query.AsNoTracking().ToListAsync();
    }

    // RF-022: proyectos sin observaciones registradas en los últimos 7 días.
    public async Task<List<Proyecto>> GetProyectosSinObservacionesRecientesAsync(int? asesorIdFiltro)
    {
        var limite = DateTime.UtcNow.AddDays(-7);

        var query = this.context.Proyectos
            .Where(p => p.EstadoProyecto != "Finalizado")
            .Where(p => !this.context.Observaciones
                .Any(o => o.ProyectoId == p.Id && o.FechaHora >= limite));

        if (asesorIdFiltro.HasValue)
            query = query.Where(p => p.AsesorId == asesorIdFiltro.Value);

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<bool> ExisteAlertaNoLeidaAsync(string tipo, int? proyectoId, int? hitoId, int usuarioDestinoId)
    {
        return await this.context.Alertas.AnyAsync(a =>
            a.Tipo == tipo &&
            a.ProyectoId == proyectoId &&
            a.HitoId == hitoId &&
            a.UsuarioDestinoId == usuarioDestinoId &&
            !a.Leida);
    }

    public async Task<List<Alerta>> GetByUsuarioDestinoAsync(int usuarioId)
    {
        return await this.context.Alertas
            .AsNoTracking()
            .Where(a => a.UsuarioDestinoId == usuarioId)
            .OrderByDescending(a => a.FechaHora)
            .ToListAsync();
    }

    public async Task<List<Alerta>> GetAlertasNoLeidasByUsuarioAsync(int usuarioId)
    {
        return await this.context.Alertas
            .AsNoTracking()
            .Where(a => a.UsuarioDestinoId == usuarioId && !a.Leida)
            .ToListAsync();
    }

    public async Task<Alerta?> GetByIdAsync(int id)
    {
        return await this.context.Alertas.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task AddAsync(Alerta alerta)
    {
        await this.context.Alertas.AddAsync(alerta);
    }

    public async Task UpdateAsync(Alerta alerta)
    {
        this.context.Alertas.Update(alerta);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await this.context.SaveChangesAsync();
    }
}