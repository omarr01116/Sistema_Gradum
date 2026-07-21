namespace SistemGradum.Application.DTOs.Proyecto;

public class CreateProyectoDto
{
    public int ClienteId { get; set; }
    public string Universidad { get; set; } = string.Empty;
    public string Carrera { get; set; } = string.Empty;
    public string Programa { get; set; } = string.Empty;
    public string TipoProyecto { get; set; } = string.Empty;
    public string Tema { get; set; } = string.Empty;
    public int AsesorId { get; set; }   // requerido por RF-005, aunque la entidad lo permita null
    public DateTime FechaInicio { get; set; }
    public DateTime FechaEntregaComprometida { get; set; }
}