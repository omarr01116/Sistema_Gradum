namespace SistemGradum.Application.DTOs.Documento;

public class DocumentoResponseDto
{
    public int Id { get; set; }
    public int ProyectoId { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string NombreArchivoOriginal { get; set; } = string.Empty;
    public int VersionActual { get; set; }
    public List<VersionDocumentoResponseDto> Versiones { get; set; } = new();
}