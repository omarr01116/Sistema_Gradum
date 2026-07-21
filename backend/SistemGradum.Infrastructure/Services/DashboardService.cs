using Microsoft.EntityFrameworkCore;
using SistemGradum.Application.DTOs.Dashboard;
using SistemGradum.Application.Interfaces;
using SistemGradum.Infrastructure.Data;

namespace SistemGradum.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly SistemGradumDbContext context;

    public DashboardService(SistemGradumDbContext context)
    {
        this.context = context;
    }

    // RF-020, sección 11: 6 indicadores para Administrador/Coordinador.
    public async Task<DashboardGeneralDto> GetGeneralAsync()
    {
        var totalClientes = await this.context.Clientes.CountAsync(c => c.Activo);
        var totalProyectos = await this.context.Proyectos.CountAsync();
        var hitosPendientes = await this.context.Hitos.CountAsync(h => h.EstadoHito == "PendienteAprobacion");
        var proyectosFinalizados = await this.context.Proyectos.CountAsync(p => p.EstadoProyecto == "Finalizado");
        var documentosCargados = await this.context.Documentos.CountAsync();

        var porEstado = await this.context.Proyectos
            .GroupBy(p => p.EstadoProyecto)
            .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
            .ToListAsync();

        return new DashboardGeneralDto
        {
            TotalClientes = totalClientes,
            TotalProyectos = totalProyectos,
            HitosPendientesAprobacion = hitosPendientes,
            ProyectosFinalizados = proyectosFinalizados,
            DocumentosCargados = documentosCargados,
            ProyectosPorEstado = porEstado.ToDictionary(x => x.Estado, x => x.Cantidad)
        };
    }

    // RF-020: para Asesor, lista de próximas entregas.
    public async Task<List<ProximaEntregaDto>> GetProximasEntregasAsync(int asesorId)
    {
        var hitos = await this.context.Hitos
            .Include(h => h.Proyecto)
            .Where(h => h.Proyecto!.AsesorId == asesorId && h.EstadoHito != "Aprobado")
            .OrderBy(h => h.FechaCompromiso)
            .Take(10)
            .AsNoTracking()
            .ToListAsync();

        return hitos.Select(h => new ProximaEntregaDto
        {
            ProyectoId = h.ProyectoId,
            CodigoProyecto = h.Proyecto!.CodigoProyecto,
            Tema = h.Proyecto.Tema,
            NombreHito = h.NombreHito,
            FechaCompromiso = h.FechaCompromiso
        }).ToList();
    }
}