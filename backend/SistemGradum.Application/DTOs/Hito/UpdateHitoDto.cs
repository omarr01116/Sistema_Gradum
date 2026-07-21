namespace SistemGradum.Application.DTOs.Hito;

public class UpdateHitoDto
{
    public string NombreHito { get; set; } = string.Empty;
    public decimal PesoPorcentual { get; set; }
    public int Orden { get; set; }
    public DateTime FechaCompromiso { get; set; }
}