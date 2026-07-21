namespace SistemGradum.Domain.Entities;

public class Documento
{
    public int Id { get; set; }
    public int ProyectoId { get; set; }
    public Proyecto? Proyecto { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public List<VersionDocumento> Versiones { get; set; } = new();
}