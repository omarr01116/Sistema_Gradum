namespace SistemGradum.Application.DTOs.Documento;

public class VersionDocumentoResponseDto
{
    public int NumeroVersion { get; set; }
    public string NombreArchivoOriginal { get; set; } = string.Empty; // <-- AGREGAR ESTA LÍNEA
    public DateTime FechaSubida { get; set; }
    public int UsuarioId { get; set; }
}