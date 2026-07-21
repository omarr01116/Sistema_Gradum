namespace SistemGradum.Application.DTOs.Observacion;

public class ObservacionResponseDto
{
    public int Id { get; set; }
    public int ProyectoId { get; set; }
    public int UsuarioId { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public DateTime FechaHora { get; set; }
    public string Detalle { get; set; } = string.Empty;
}