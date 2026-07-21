namespace SistemGradum.Application.DTOs.Hito;

public class CreateHitosLoteDto
{
    public List<CreateHitoItemDto> Hitos { get; set; } = new();
}