namespace SistemGradum.Application.DTOs.Alerta;

public class AlertaResponseDto
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public int? ProyectoId { get; set; }
    public int? HitoId { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public DateTime FechaHora { get; set; }
    public bool Leida { get; set; }
}