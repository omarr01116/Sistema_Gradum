namespace SistemGradum.Application.DTOs;

public class UpdateClienteDto
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string EstadoFinanciero  { get; set; } = string.Empty; 
}