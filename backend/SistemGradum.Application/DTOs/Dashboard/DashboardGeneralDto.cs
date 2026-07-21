namespace SistemGradum.Application.DTOs.Dashboard;

public class DashboardGeneralDto
{
    public int TotalClientes { get; set; }
    public int TotalProyectos { get; set; }
    public int HitosPendientesAprobacion { get; set; }
    public int ProyectosFinalizados { get; set; }
    public int DocumentosCargados { get; set; }
    public Dictionary<string, int> ProyectosPorEstado { get; set; } = new();
}