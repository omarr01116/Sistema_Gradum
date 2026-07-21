using SistemGradum.Application.DTOs.Alerta;
using SistemGradum.Application.Interfaces;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Services;

public class AlertaService : IAlertaService
{
    private readonly IAlertaRepository alertaRepository;

    public AlertaService(IAlertaRepository alertaRepository)
    {
        this.alertaRepository = alertaRepository;
    }

    // RF-022: se ejecuta al iniciar sesión, no como proceso en segundo plano.
    public async Task GenerarAlertasAsync(int usuarioId, string rol, int? asesorIdFiltro)
    {
        // Solo Coordinador, Administrador y Asesor reciben alertas de proyectos/hitos.
        // RN-08 ya limita qué proyectos ve cada uno vía asesorIdFiltro.

        var alertasNoLeidas = await this.alertaRepository.GetAlertasNoLeidasByUsuarioAsync(usuarioId);
        var unreadSet = new HashSet<(string Tipo, int? ProyectoId, int? HitoId)>(
            alertasNoLeidas.Select(a => (a.Tipo, a.ProyectoId, a.HitoId)));

        var hitosProximos = await this.alertaRepository.GetHitosProximosAVencerAsync(asesorIdFiltro);
        foreach (var hito in hitosProximos)
        {
            var yaExiste = unreadSet.Contains(("HitoProximoAVencer", hito.ProyectoId, hito.Id));

            if (!yaExiste)
            {
                await this.alertaRepository.AddAsync(new Alerta
                {
                    Tipo = "HitoProximoAVencer",
                    ProyectoId = hito.ProyectoId,
                    HitoId = hito.Id,
                    UsuarioDestinoId = usuarioId,
                    Mensaje = $"El hito '{hito.NombreHito}' vence el {hito.FechaCompromiso:yyyy-MM-dd}.",
                    FechaHora = DateTime.UtcNow,
                    Leida = false
                });
            }
        }

        var proyectosEnCorrecciones = await this.alertaRepository.GetProyectosEnCorreccionesAsync(asesorIdFiltro);
        foreach (var proyecto in proyectosEnCorrecciones)
        {
            var yaExiste = unreadSet.Contains(("ProyectoEnCorrecciones", proyecto.Id, null));

            if (!yaExiste)
            {
                await this.alertaRepository.AddAsync(new Alerta
                {
                    Tipo = "ProyectoEnCorrecciones",
                    ProyectoId = proyecto.Id,
                    UsuarioDestinoId = usuarioId,
                    Mensaje = $"El proyecto '{proyecto.CodigoProyecto}' está en estado Correcciones.",
                    FechaHora = DateTime.UtcNow,
                    Leida = false
                });
            }
        }

        var proyectosSinActividad = await this.alertaRepository.GetProyectosSinObservacionesRecientesAsync(asesorIdFiltro);
        foreach (var proyecto in proyectosSinActividad)
        {
            var yaExiste = unreadSet.Contains(("SinActividadReciente", proyecto.Id, null));

            if (!yaExiste)
            {
                await this.alertaRepository.AddAsync(new Alerta
                {
                    Tipo = "SinActividadReciente",
                    ProyectoId = proyecto.Id,
                    UsuarioDestinoId = usuarioId,
                    Mensaje = $"El proyecto '{proyecto.CodigoProyecto}' no tiene observaciones en los últimos 7 días.",
                    FechaHora = DateTime.UtcNow,
                    Leida = false
                });
            }
        }

        await this.alertaRepository.SaveChangesAsync();
    }

    public async Task<List<AlertaResponseDto>> GetByUsuarioAsync(int usuarioId)
    {
        var alertas = await this.alertaRepository.GetByUsuarioDestinoAsync(usuarioId);
        return alertas.Select(MapToResponse).ToList();
    }

    public async Task<(bool Success, string? Error)> MarcarLeidaAsync(int id, int usuarioId)
    {
        var alerta = await this.alertaRepository.GetByIdAsync(id);
        if (alerta is null || alerta.UsuarioDestinoId != usuarioId)
            return (false, "Alerta no encontrada.");

        alerta.Leida = true;

        await this.alertaRepository.UpdateAsync(alerta);
        await this.alertaRepository.SaveChangesAsync();

        return (true, null);
    }

    private static AlertaResponseDto MapToResponse(Alerta a) => new()
    {
        Id = a.Id,
        Tipo = a.Tipo,
        ProyectoId = a.ProyectoId,
        HitoId = a.HitoId,
        Mensaje = a.Mensaje,
        FechaHora = a.FechaHora,
        Leida = a.Leida
    };
}