namespace SistemGradum.Domain.Entities;

public class VersionDocumento
{
    public int Id { get; set; }

    public int DocumentoId { get; set; }
    public Documento? Documento { get; set; }

    public int NumeroVersion { get; set; }
    public string RutaArchivo { get; set; } = string.Empty;
    public DateTime FechaSubida { get; set; } = DateTime.UtcNow;
    public int UsuarioId { get; set; }
}