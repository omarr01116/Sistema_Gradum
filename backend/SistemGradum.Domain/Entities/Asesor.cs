namespace SistemGradum.Domain.Entities;

public class Asesor
{
    public int Id { get; set; }
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // String simple: fuera de alcance un catálogo de líneas de investigación (RF-018)
    public string Especialidad { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    // Navegación inversa 1:1 opcional -> Usuario.AsesorId (UNIQUE)
    public Usuario? Usuario { get; set; }

    // Navegación 1:N -> un Asesor puede estar asignado a múltiples Proyectos
    public ICollection<Proyecto> Proyectos { get; set; } = new List<Proyecto>();
}