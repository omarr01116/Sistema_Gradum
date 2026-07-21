namespace SistemGradum.Application.DTOs.Usuario;

public class CreateUsuarioDto
{
    public string NombreUsuario { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty; // "Administrador" | "Coordinador" | "Asesor"
    public int? AsesorId { get; set; } // obligatorio solo si Rol == "Asesor"
}