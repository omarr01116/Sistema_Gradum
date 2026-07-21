namespace SistemGradum.Domain.Entities;

public class Observacion
{
    public int Id { get; set; }

    public int ProyectoId { get; set; }
    public Proyecto? Proyecto { get; set; }

    public int UsuarioId { get; set; }

    public DateTime FechaHora { get; set; } = DateTime.UtcNow;
    public string Detalle { get; set; } = string.Empty;
}