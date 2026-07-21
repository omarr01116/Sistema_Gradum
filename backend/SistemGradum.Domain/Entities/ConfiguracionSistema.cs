namespace SistemGradum.Domain.Entities;

public class ConfiguracionSistema
{
    public int Id { get; set; }

    // UNIQUE (configurado en Infrastructure). Ej: "LIMITE_PROYECTOS_ASESOR"
    public string Clave { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}