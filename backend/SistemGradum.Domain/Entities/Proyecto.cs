namespace SistemGradum.Domain.Entities;

public class Proyecto
{
    public int Id { get; set; }
    public string CodigoProyecto { get; set; } = string.Empty;

    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public int? AsesorId { get; set; }
    public Asesor? Asesor { get; set; }

    public string Universidad { get; set; } = string.Empty;
    public string Carrera { get; set; } = string.Empty;
    public string Programa { get; set; } = string.Empty;
    public string TipoProyecto { get; set; } = string.Empty;
    public string Tema { get; set; } = string.Empty;

    public DateTime FechaInicio { get; set; }
    public DateTime FechaEntregaComprometida { get; set; }

    // Valores esperados (máquina de estados, sección 8):
    // "Activo" | "Pausado" | "Correcciones" | "Sustentado" | "Finalizado"
    public string EstadoProyecto { get; set; } = "Activo";

    // Trazabilidad embebida del último cambio de estado (RN-06).
    // Reemplaza la tabla AuditoriaEstado eliminada del alcance: se pierde
    // el historial completo de transiciones, pero se conserva quién y
    // cuándo fue el último cambio, que es lo que este alcance requiere.
    public int? UsuarioUltimoCambioId { get; set; }
    public DateTime? FechaUltimoCambio { get; set; }

    public ICollection<Hito> Hitos { get; set; } = new List<Hito>();
    public ICollection<Observacion> Observaciones { get; set; } = new List<Observacion>();
    public ICollection<Documento> Documentos { get; set; } = new List<Documento>();
}