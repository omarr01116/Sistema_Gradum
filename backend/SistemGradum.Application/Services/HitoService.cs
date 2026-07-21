using SistemGradum.Application.DTOs.Hito;
using SistemGradum.Application.Exceptions;
using SistemGradum.Application.Interfaces;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Services;

public class HitoService : IHitoService
{
    private readonly IHitoRepository hitoRepository;
    private readonly IProyectoRepository proyectoRepository;

    public HitoService(IHitoRepository hitoRepository, IProyectoRepository proyectoRepository)
    {
        this.hitoRepository = hitoRepository;
        this.proyectoRepository = proyectoRepository;
    }

    public async Task<List<HitoResponseDto>?> GetByProyectoIdAsync(int proyectoId, int? asesorIdFiltro)
    {
        var proyecto = await this.proyectoRepository.GetByIdAsync(proyectoId);
        if (proyecto is null)
            return null;

        if (asesorIdFiltro.HasValue && proyecto.AsesorId != asesorIdFiltro.Value)
            return null;

        var hitos = await this.hitoRepository.GetByProyectoIdAsync(proyectoId);
        return hitos.Select(MapToResponse).ToList();
    }

    public async Task<List<HitoResponseDto>> CrearLoteAsync(int proyectoId, CreateHitosLoteDto dto)
    {
        var proyecto = await this.proyectoRepository.GetByIdAsync(proyectoId);
        if (proyecto is null)
            throw new ReglaNegocioException("El proyecto indicado no existe.");

        if (dto.Hitos.Count == 0)
            throw new ReglaNegocioException("Debe enviar al menos un hito.");

        var hitosExistentes = await this.hitoRepository.GetByProyectoIdAsync(proyectoId);
        if (hitosExistentes.Count > 0)
            throw new ReglaNegocioException(
                "El proyecto ya tiene hitos definidos. Use PUT /api/hito/{id} para editarlos individualmente.");

        var suma = dto.Hitos.Sum(h => h.PesoPorcentual);
        if (suma != 100)
            throw new ReglaNegocioException(
                $"La suma de los pesos de los hitos debe ser exactamente 100%. Suma enviada: {suma}%.");

        var hitos = dto.Hitos.Select(item => new Hito
        {
            ProyectoId = proyectoId,
            NombreHito = item.NombreHito,
            PesoPorcentual = item.PesoPorcentual,
            Orden = item.Orden,
            FechaCompromiso = item.FechaCompromiso,
            EstadoHito = "Pendiente"
        }).ToList();

        await this.hitoRepository.AddRangeAsync(hitos);
        await this.hitoRepository.SaveChangesAsync();

        return hitos.Select(MapToResponse).ToList();
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(int id, UpdateHitoDto dto)
    {
        var hito = await this.hitoRepository.GetByIdAsync(id);
        if (hito is null)
            return (false, "Hito no encontrado.");

        if (hito.PesoPorcentual != dto.PesoPorcentual)
        {
            if (hito.EstadoHito == "Aprobado")
                return (false, "No se puede modificar el peso de un hito ya aprobado.");

            var otrosHitos = await this.hitoRepository.GetByProyectoIdAsync(hito.ProyectoId);
            var sumaSinEste = otrosHitos.Where(h => h.Id != id).Sum(h => h.PesoPorcentual);
            var nuevaSuma = sumaSinEste + dto.PesoPorcentual;

            if (nuevaSuma != 100)
                return (false,
                    $"El nuevo peso dejaría la suma total en {nuevaSuma}%. Debe ser exactamente 100%.");
        }

        hito.NombreHito = dto.NombreHito;
        hito.PesoPorcentual = dto.PesoPorcentual;
        hito.Orden = dto.Orden;
        hito.FechaCompromiso = dto.FechaCompromiso;

        await this.hitoRepository.UpdateAsync(hito);
        await this.hitoRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var hito = await this.hitoRepository.GetByIdAsync(id);
        if (hito is null)
            return (false, "Hito no encontrado.");

        var yaTieneAprobados = await this.hitoRepository.ExisteHitoAprobadoEnProyectoAsync(hito.ProyectoId);
        if (yaTieneAprobados)
            return (false,
                "No se pueden eliminar hitos de un proyecto que ya tiene hitos aprobados.");

        await this.hitoRepository.DeleteAsync(hito);
        await this.hitoRepository.SaveChangesAsync();

        return (true, null);
    }

    // RF-009: el Asesor marca el hito como completado.
    public async Task<(bool Success, string? Error)> CompletarAsync(
        int id, int? documentoEvidenciaId, int usuarioId, int? asesorIdFiltro)
    {
        var hito = await this.hitoRepository.GetByIdAsync(id);
        if (hito is null)
            return (false, "Hito no encontrado.");

        var proyecto = await this.proyectoRepository.GetByIdAsync(hito.ProyectoId);
        if (proyecto is null)
            return (false, "El proyecto asociado a este hito no existe.");

        // RN-08: el Asesor solo puede completar hitos de proyectos donde es el asignado.
        if (asesorIdFiltro.HasValue && proyecto.AsesorId != asesorIdFiltro.Value)
            return (false, "Hito no encontrado.");

        if (hito.EstadoHito is not ("Pendiente" or "EnProgreso"))
            return (false,
                $"No se puede completar un hito en estado '{hito.EstadoHito}'. Debe estar en 'Pendiente' o 'EnProgreso'.");

        hito.EstadoHito = "PendienteAprobacion";
        hito.UsuarioCompletadoId = usuarioId;
        hito.FechaCompletado = DateTime.UtcNow;
        hito.DocumentoEvidenciaId = documentoEvidenciaId;

        await this.hitoRepository.UpdateAsync(hito);
        await this.hitoRepository.SaveChangesAsync();

        return (true, null);
    }

    // RF-010 / RN-05: el Coordinador aprueba el hito.
    public async Task<(bool Success, string? Error)> AprobarAsync(int id, int usuarioId)
    {
        var hito = await this.hitoRepository.GetByIdAsync(id);
        if (hito is null)
            return (false, "Hito no encontrado.");

        if (hito.EstadoHito != "PendienteAprobacion")
            return (false,
                $"No se puede aprobar un hito en estado '{hito.EstadoHito}'. Debe estar en 'PendienteAprobacion'.");

        hito.EstadoHito = "Aprobado";
        hito.UsuarioAprobadorId = usuarioId;
        hito.FechaAprobacion = DateTime.UtcNow;
        hito.RazonRechazo = null;

        await this.hitoRepository.UpdateAsync(hito);
        await this.hitoRepository.SaveChangesAsync();

        // RF-008: el recálculo de avance no se guarda aquí — se calcula al vuelo
        // cada vez que se consulta el Proyecto (ver ProyectoService.MapToResponseAsync).

        return (true, null);
    }

    // RF-010 / RN-05: el Coordinador rechaza el hito, con motivo obligatorio.
    public async Task<(bool Success, string? Error)> RechazarAsync(int id, RechazarHitoDto dto)
    {
        var hito = await this.hitoRepository.GetByIdAsync(id);
        if (hito is null)
            return (false, "Hito no encontrado.");

        if (hito.EstadoHito != "PendienteAprobacion")
            return (false,
                $"No se puede rechazar un hito en estado '{hito.EstadoHito}'. Debe estar en 'PendienteAprobacion'.");

        if (string.IsNullOrWhiteSpace(dto.Motivo))
            return (false, "Debe indicar el motivo del rechazo.");

        // RN-05: regresa a "EnProgreso"; el asesor deberá completarlo de nuevo.
        hito.EstadoHito = "EnProgreso";
        hito.RazonRechazo = dto.Motivo;
        hito.UsuarioCompletadoId = null;
        hito.FechaCompletado = null;

        await this.hitoRepository.UpdateAsync(hito);
        await this.hitoRepository.SaveChangesAsync();

        return (true, null);
    }

    // RF-011: obtiene solo el ProyectoId de un hito, usado por el Controller
    // para saber en qué proyecto/carpeta guardar la evidencia antes de completar.
    public async Task<int?> ObtenerProyectoIdAsync(int hitoId)
    {
        var hito = await this.hitoRepository.GetByIdAsync(hitoId);
        return hito?.ProyectoId;
    }

    private static HitoResponseDto MapToResponse(Hito h) => new()
    {
        Id = h.Id,
        ProyectoId = h.ProyectoId,
        NombreHito = h.NombreHito,
        PesoPorcentual = h.PesoPorcentual,
        Orden = h.Orden,
        FechaCompromiso = h.FechaCompromiso,
        EstadoHito = h.EstadoHito,
        UsuarioCompletadoId = h.UsuarioCompletadoId,
        FechaCompletado = h.FechaCompletado,
        UsuarioAprobadorId = h.UsuarioAprobadorId,
        FechaAprobacion = h.FechaAprobacion,
        RazonRechazo = h.RazonRechazo,
        DocumentoEvidenciaId = h.DocumentoEvidenciaId
    };
}