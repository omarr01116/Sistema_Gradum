using System.ComponentModel.DataAnnotations;

namespace SistemGradum.domain.Entities;
public class Cliente
{
    public int Id { get; set; }
    public string CodigoCliente { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string DniPasaporte { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set;} = string.Empty;
    public string EstadoFinanciero { get; set; }  = string.Empty;
    public bool Activo { get; set; } = true;

}