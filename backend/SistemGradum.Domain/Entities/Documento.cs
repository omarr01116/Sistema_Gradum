namespace SistemGradum.Domain.Entities;

public class Documento
{
    public int Id { get; set; }

    public int ProyectoId { get; set; }
    public Proyecto? Proyecto { get; set; }

    public string Categoria { get; set; } = string.Empty;
    public string NombreArchivoOriginal { get; set; } = string.Empty;

    // Denormalización deliberada: evita un MAX(NumeroVersion) en cada
    // listado de documentos. Se actualiza en el mismo caso de uso que
    // crea una nueva VersionDocumento, así que no rompe consistencia.
    public int VersionActual { get; set; } = 1;

    // Historial completo obligatorio (RN-07): no se pueden eliminar versiones.
    public ICollection<VersionDocumento> Versiones { get; set; } = new List<VersionDocumento>();
}