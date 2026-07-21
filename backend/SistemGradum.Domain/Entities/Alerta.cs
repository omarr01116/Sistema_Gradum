namespace SistemGradum.Domain.Entities;

public class Alerta
{
    public int Id { get; set; }

    // Valores esperados: "HitoProximoAVencer" | "ProyectoEnCorrecciones" | "SinActividadReciente"
    public string Tipo { get; set; } = string.Empty;

    // Asociación opcional a Proyecto, a Hito, o a ambos según el tipo (sección 10.1)
    public int? ProyectoId { get; set; }
    public int? HitoId { get; set; }

    public int UsuarioDestinoId { get; set; }

    public string Mensaje { get; set; } = string.Empty;
    public DateTime FechaHora { get; set; } = DateTime.UtcNow;
    public bool Leida { get; set; } = false;
}