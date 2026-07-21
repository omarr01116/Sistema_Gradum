namespace SistemGradum.Application.DTOs.Proyecto;

public class ProyectoResponseDto
{
    public int Id { get; set; }
    public string CodigoProyecto { get; set; } = string.Empty;
    public int ClienteId { get; set; }
    public string Universidad { get; set; } = string.Empty;
    public string Carrera { get; set; } = string.Empty;
    public string Programa { get; set; } = string.Empty;
    public string TipoProyecto { get; set; } = string.Empty;
    public string Tema { get; set; } = string.Empty;
    public int? AsesorId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaEntregaComprometida { get; set; }
    public string EstadoProyecto { get; set; } = string.Empty;
    public DateTime? FechaUltimoCambio { get; set; }

    public decimal PorcentajeAvance { get; set; }   // ← nuevo, RF-008 
}