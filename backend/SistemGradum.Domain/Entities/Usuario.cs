namespace SistemGradum.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    // Valores esperados: "Administrador" | "Coordinador" | "Asesor"
    public string Rol { get; set; } = string.Empty;

    // FK opcional + UNIQUE (configurado en Infrastructure): 1:1 con Asesor
    // solo se asigna cuando Rol == "Asesor"
    public int? AsesorId { get; set; }
    public Asesor? Asesor { get; set; }

    public bool Activo { get; set; } = true;
}