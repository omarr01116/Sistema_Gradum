using Microsoft.AspNetCore.Http;

namespace SistemGradum.Application.DTOs.Hito;

public class CompletarHitoDto
{
    public IFormFile? Evidencia { get; set; }
}