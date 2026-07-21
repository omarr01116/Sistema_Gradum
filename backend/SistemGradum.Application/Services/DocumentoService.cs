using Microsoft.AspNetCore.Http;
using SistemGradum.Application.DTOs.Documento;
using SistemGradum.Application.Interfaces;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Services;

public class DocumentoService : IDocumentoService
{
    private readonly IDocumentoRepository documentoRepository;
    private readonly IProyectoRepository proyectoRepository;
    private readonly IAlmacenamientoArchivos almacenamiento;

    public DocumentoService(
        IDocumentoRepository documentoRepository,
        IProyectoRepository proyectoRepository,
        IAlmacenamientoArchivos almacenamiento)
    {
        this.documentoRepository = documentoRepository;
        this.proyectoRepository = proyectoRepository;
        this.almacenamiento = almacenamiento;
    }

    public async Task<(bool, string?, DocumentoResponseDto?)> SubirAsync(
        int proyectoId, string categoria, IFormFile archivo, int usuarioId, int? asesorIdFiltro)
    {
        if (archivo is null || archivo.Length == 0)
            return (false, "Debe adjuntar un archivo.", null);

        var proyecto = await this.proyectoRepository.GetByIdAsync(proyectoId);
        if (proyecto is null)
            return (false, "Proyecto no encontrado.", null);

        // RN-08: el Asesor solo sube documentos de sus proyectos asignados.
        if (asesorIdFiltro.HasValue && proyecto.AsesorId != asesorIdFiltro.Value)
            return (false, "Proyecto no encontrado.", null);

        var nombreOriginal = Path.GetFileName(archivo.FileName);
        var documento = await this.documentoRepository.GetByProyectoIdYCategoriaAsync(proyectoId, categoria);

        int numeroVersion;

        if (documento is null)
        {
            documento = new Documento
            {
                ProyectoId = proyectoId,
                Categoria = categoria
            };

            await this.documentoRepository.AddDocumentoAsync(documento);
            await this.documentoRepository.SaveChangesAsync();

            numeroVersion = 1;
        }
        else
        {
            // Calcula la versión en base al historial de la tabla hija
            numeroVersion = documento.Versiones.Count > 0 ? documento.Versiones.Max(v => v.NumeroVersion) + 1 : 1;
        }

        var rutaRelativa = await this.almacenamiento.GuardarAsync(proyectoId, categoria, archivo);

        var version = new VersionDocumento
        {
            DocumentoId = documento.Id,
            NumeroVersion = numeroVersion,
            NombreArchivoOriginal = nombreOriginal, 
            RutaArchivo = rutaRelativa,
            UsuarioId = usuarioId
        };

        await this.documentoRepository.AddVersionAsync(version);
        await this.documentoRepository.SaveChangesAsync();

        // Evita un NullReferenceException antes de agregar la versión
        if (documento.Versiones == null) 
            documento.Versiones = new List<VersionDocumento>();
            
        documento.Versiones.Add(version);

        return (true, null, this.MapToResponse(documento));
    }

    public async Task<(bool, string?, DocumentoResponseDto?)> GetVersionesAsync(int documentoId, int? asesorIdFiltro)
    {
        var documento = await this.documentoRepository.GetByIdConVersionesAsync(documentoId);
        if (documento is null)
            return (false, "Documento no encontrado.", null);

        if (asesorIdFiltro.HasValue && documento.Proyecto?.AsesorId != asesorIdFiltro.Value)
            return (false, "Documento no encontrado.", null);

        return (true, null, this.MapToResponse(documento));
    }

    public async Task<(bool, string?, string?, string?)> ObtenerParaDescargaAsync(
        int documentoId, int numeroVersion, int? asesorIdFiltro)
    {
        var documento = await this.documentoRepository.GetByIdConVersionesAsync(documentoId);
        if (documento is null)
            return (false, "Documento no encontrado.", null, null);

        if (asesorIdFiltro.HasValue && documento.Proyecto?.AsesorId != asesorIdFiltro.Value)
            return (false, "Documento no encontrado.", null, null);

        var version = documento.Versiones.FirstOrDefault(v => v.NumeroVersion == numeroVersion);
        if (version is null)
            return (false, "Versión no encontrada.", null, null);

        var rutaCompleta = this.almacenamiento.ObtenerRutaCompleta(version.RutaArchivo);
        if (!File.Exists(rutaCompleta))
            return (false, "El archivo ya no existe en el almacenamiento.", null, null);

        return (true, null, rutaCompleta, version.NombreArchivoOriginal); 
    }

    private DocumentoResponseDto MapToResponse(Documento d) 
    {
        // Buscamos la última versión para llenar los datos raíz que espera tu DTO
        var ultimaVersion = d.Versiones?.OrderByDescending(v => v.NumeroVersion).FirstOrDefault();

        return new DocumentoResponseDto
        {
            Id = d.Id,
            ProyectoId = d.ProyectoId,
            Categoria = d.Categoria,
            
            // Aquí está la corrección clave: mapeamos dinámicamente
            NombreArchivoOriginal = ultimaVersion?.NombreArchivoOriginal ?? "",
            VersionActual = ultimaVersion?.NumeroVersion ?? 0,
            
            Versiones = d.Versiones != null 
                ? d.Versiones.OrderByDescending(v => v.NumeroVersion)
                             .Select(v => new VersionDocumentoResponseDto
                             {
                                 NumeroVersion = v.NumeroVersion,
                                 NombreArchivoOriginal = v.NombreArchivoOriginal,
                                 FechaSubida = v.FechaSubida,
                                 UsuarioId = v.UsuarioId
                             }).ToList()
                : new List<VersionDocumentoResponseDto>()
        };
    }
}