namespace SistemGradum.Application.DTOs.Proyecto;

public class UpdateProyectoDto
{
    public string Universidad { get; set; } = string.Empty;
    public string Carrera { get; set; } = string.Empty;
    public string Programa { get; set; } = string.Empty;
    public string TipoProyecto { get; set; } = string.Empty;
    public string Tema { get; set; } = string.Empty;
    public int AsesorId { get; set; }
    public DateTime FechaEntregaComprometida { get; set; }
}