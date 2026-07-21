using Microsoft.AspNetCore.Http;
using SistemGradum.Application.DTOs.Documento;

namespace SistemGradum.Application.Interfaces;

public interface IDocumentoService
{
    Task<(bool Success, string? Error, DocumentoResponseDto? Resultado)> SubirAsync(
        int proyectoId, string categoria, IFormFile archivo, int usuarioId, int? asesorIdFiltro);

    Task<(bool Success, string? Error, DocumentoResponseDto? Resultado)> GetVersionesAsync(
        int documentoId, int? asesorIdFiltro);

    Task<(bool Success, string? Error, string? RutaCompleta, string? NombreDescarga)> ObtenerParaDescargaAsync(
        int documentoId, int numeroVersion, int? asesorIdFiltro);
}