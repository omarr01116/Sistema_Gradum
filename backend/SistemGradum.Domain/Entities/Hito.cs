namespace SistemGradum.Domain.Entities;

public class Hito
{
    public int Id { get; set; }

    public int ProyectoId { get; set; }
    public Proyecto? Proyecto { get; set; }

    public string NombreHito { get; set; } = string.Empty;

    // Suma de PesoPorcentual de todos los hitos de un proyecto debe ser 100% (RN-04).
    // Esa invariante se valida en Application al crear/editar hitos, no aquí.
    public decimal PesoPorcentual { get; set; }

    public int Orden { get; set; }
    public DateTime FechaCompromiso { get; set; }

    // Valores esperados (sección 9):
    // "Pendiente" | "EnProgreso" | "PendienteAprobacion" | "Aprobado"
    public string EstadoHito { get; set; } = "Pendiente";

    // Trazabilidad embebida (RF-009/RF-010). Reemplaza la tabla
    // HistorialRechazoHito eliminada del alcance.
    public int? UsuarioCompletadoId { get; set; }
    public DateTime? FechaCompletado { get; set; }
    public int? UsuarioAprobadorId { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public string? RazonRechazo { get; set; }
}