namespace SistemGradum.Application.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public int? AsesorId { get; set; }
    public DateTime ExpiraEn { get; set; }
}