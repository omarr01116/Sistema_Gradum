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

        // RN-08: un Asesor solo ve hitos de sus propios proyectos.
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

        // RN-04: la suma de pesos debe ser exactamente 100%.
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

            // RN-04: recalcular la suma total del proyecto con el nuevo peso.
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

        // No se elimina si el proyecto ya tiene algún hito aprobado
        // (rompería el cálculo de avance ya realizado, RF-008).
        var yaTieneAprobados = await this.hitoRepository.ExisteHitoAprobadoEnProyectoAsync(hito.ProyectoId);
        if (yaTieneAprobados)
            return (false,
                "No se pueden eliminar hitos de un proyecto que ya tiene hitos aprobados.");

        await this.hitoRepository.DeleteAsync(hito);
        await this.hitoRepository.SaveChangesAsync();

        return (true, null);
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
        RazonRechazo = h.RazonRechazo
    };
}