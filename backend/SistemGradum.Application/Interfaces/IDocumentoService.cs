using Microsoft.AspNetCore.Http;
using SistemGradum.Application.DTOs.Documento;

namespace SistemGradum.Application.Interfaces;

public interface IDocumentoService
{
    Task<(bool Success, string? Error, DocumentoResponseDto? Resultado)> SubirAsync(
        int proyectoId, string categoria, IFormFile archivo, int usuarioId, int? asesorIdFiltro);

    Task<(bool Success, string? Error, DocumentoResponseDto? Resultado)> GetVersionesAsync(
        int documentoId, int? asesorIdFiltro);

    Task<(bool Success, string? Error, System.IO.Stream? Contenido, string? NombreDescarga)> ObtenerParaDescargaAsync(
        int documentoId, int numeroVersion, int? asesorIdFiltro);
        
    Task<List<DocumentoResponseDto>?> GetByProyectoIdAsync(int proyectoId, int? asesorIdFiltro);
}