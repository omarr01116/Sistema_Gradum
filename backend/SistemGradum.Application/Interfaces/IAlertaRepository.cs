using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Interfaces;

public interface IAlertaRepository
{
    Task<List<Hito>> GetHitosProximosAVencerAsync(int? asesorIdFiltro);
    Task<List<Proyecto>> GetProyectosEnCorreccionesAsync(int? asesorIdFiltro);
    Task<List<Proyecto>> GetProyectosSinObservacionesRecientesAsync(int? asesorIdFiltro);
    Task<bool> ExisteAlertaNoLeidaAsync(string tipo, int? proyectoId, int? hitoId, int usuarioDestinoId);
    Task<List<Alerta>> GetByUsuarioDestinoAsync(int usuarioId);
    Task<List<Alerta>> GetAlertasNoLeidasByUsuarioAsync(int usuarioId);
    Task<Alerta?> GetByIdAsync(int id);
    Task AddAsync(Alerta alerta);
    Task UpdateAsync(Alerta alerta);
    Task SaveChangesAsync();
}