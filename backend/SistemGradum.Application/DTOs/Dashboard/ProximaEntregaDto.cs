namespace SistemGradum.Application.DTOs.Dashboard;

public class ProximaEntregaDto
{
    public int ProyectoId { get; set; }
    public string CodigoProyecto { get; set; } = string.Empty;
    public string Tema { get; set; } = string.Empty;
    public string NombreHito { get; set; } = string.Empty;
    public DateTime FechaCompromiso { get; set; }
}