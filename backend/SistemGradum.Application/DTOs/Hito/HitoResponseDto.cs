namespace SistemGradum.Application.DTOs.Hito;

public class HitoResponseDto
{
    public int Id { get; set; }
    public int ProyectoId { get; set; }
    public string NombreHito { get; set; } = string.Empty;
    public decimal PesoPorcentual { get; set; }
    public int Orden { get; set; }
    public DateTime FechaCompromiso { get; set; }
    public string EstadoHito { get; set; } = string.Empty;
    public int? UsuarioCompletadoId { get; set; }
    public DateTime? FechaCompletado { get; set; }
    public int? UsuarioAprobadorId { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public string? RazonRechazo { get; set; }

    public string? RutaEvidenciaTemporal { get; set; }
}