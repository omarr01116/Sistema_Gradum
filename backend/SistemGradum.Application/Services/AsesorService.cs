using SistemGradum.Application.DTOs.Asesor;
using SistemGradum.Application.Interfaces;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Services;

public class AsesorService : IAsesorService
{
    private readonly IAsesorRepository asesorRepository;

    public AsesorService(IAsesorRepository asesorRepository)
    {
        this.asesorRepository = asesorRepository;
    }

    public async Task<List<AsesorResponseDto>> GetAllAsync()
{
    var asesores = await this.asesorRepository.GetAllAsync();

    return asesores
        .Select(MapToResponse)
        .ToList();
}

    public async Task<AsesorResponseDto?> GetByIdAsync(int id)
    {
        var asesor = await this.asesorRepository.GetByIdAsync(id);

        return asesor is null || !asesor.Activo
            ? null
            : MapToResponse(asesor);
    }

    public async Task<AsesorResponseDto> CreateAsync(CreateAsesorDto dto)
    {
        var asesor = new Asesor
        {
            Nombres = dto.Nombres,
            Apellidos = dto.Apellidos,
            Telefono = dto.Telefono,
            Email = dto.Email,
            Especialidad = dto.Especialidad,
            Activo = true
        };

        await this.asesorRepository.AddAsync(asesor);
        await this.asesorRepository.SaveChangesAsync();

        return MapToResponse(asesor);
    }

    public async Task<bool> UpdateAsync(int id, UpdateAsesorDto dto)
    {
        var asesor = await this.asesorRepository.GetByIdAsync(id);

        if (asesor is null || !asesor.Activo)
            return false;

        asesor.Nombres = dto.Nombres;
        asesor.Apellidos = dto.Apellidos;
        asesor.Telefono = dto.Telefono;
        asesor.Email = dto.Email;
        asesor.Especialidad = dto.Especialidad;

        await this.asesorRepository.UpdateAsync(asesor);
        await this.asesorRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var asesor = await this.asesorRepository.GetByIdAsync(id);

        if (asesor is null || !asesor.Activo)
            return false;

        asesor.Activo = false;

        await this.asesorRepository.UpdateAsync(asesor);
        await this.asesorRepository.SaveChangesAsync();

        return true;
    }

    private static AsesorResponseDto MapToResponse(Asesor asesor)
    {
        return new AsesorResponseDto
        {
            Id = asesor.Id,
            Nombres = asesor.Nombres,
            Apellidos = asesor.Apellidos,
            Telefono = asesor.Telefono,
            Email = asesor.Email,
            Especialidad = asesor.Especialidad,
            Activo = asesor.Activo
        };
    }

    public async Task<bool> ReactivarAsync(int id)
{
    var asesor = await this.asesorRepository.GetByIdAsync(id);

    if (asesor is null || asesor.Activo)
        return false;   // no existe, o ya estaba activo

    asesor.Activo = true;

    await this.asesorRepository.UpdateAsync(asesor);
    await this.asesorRepository.SaveChangesAsync();

    return true;
}
}