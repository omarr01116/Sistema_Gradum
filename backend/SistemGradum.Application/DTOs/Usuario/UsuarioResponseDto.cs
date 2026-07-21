namespace SistemGradum.Application.DTOs.Usuario;

public class UsuarioResponseDto
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public int? AsesorId { get; set; }
    public bool Activo { get; set; }
}