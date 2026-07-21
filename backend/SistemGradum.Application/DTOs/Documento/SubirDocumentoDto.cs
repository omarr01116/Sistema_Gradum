using Microsoft.AspNetCore.Http;

namespace SistemGradum.Application.DTOs.Documento;

public class SubirDocumentoDto
{
    public string Categoria { get; set; } = string.Empty;
    public IFormFile Archivo { get; set; } = null!;
}