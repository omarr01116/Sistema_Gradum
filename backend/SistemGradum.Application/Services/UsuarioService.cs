using BCrypt.Net;
using SistemGradum.Application.DTOs.Usuario;
using SistemGradum.Application.Interfaces;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository usuarioRepository;
    private readonly IAsesorRepository asesorRepository;

    private static readonly string[] RolesValidos = { "Administrador", "Coordinador", "Asesor" };

    public UsuarioService(IUsuarioRepository usuarioRepository, IAsesorRepository asesorRepository)
    {
        this.usuarioRepository = usuarioRepository;
        this.asesorRepository = asesorRepository;
    }

    public async Task<List<UsuarioResponseDto>> GetAllAsync()
    {
        var usuarios = await this.usuarioRepository.GetAllAsync();
        return usuarios.Select(MapToResponse).ToList();
    }

    public async Task<UsuarioResponseDto?> GetByIdAsync(int id)
    {
        var usuario = await this.usuarioRepository.GetByIdAsync(id);
        return usuario is null ? null : MapToResponse(usuario);
    }

    public async Task<(UsuarioResponseDto? Usuario, string? Error)> CreateAsync(CreateUsuarioDto dto)
    {
        if (!RolesValidos.Contains(dto.Rol))
            return (null, "El rol debe ser 'Administrador', 'Coordinador' o 'Asesor'.");

        if (string.IsNullOrWhiteSpace(dto.NombreUsuario))
            return (null, "El nombre de usuario es obligatorio.");

        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
            return (null, "La contraseña debe tener al menos 6 caracteres.");

        var existente = await this.usuarioRepository.GetByUsernameIncludingInactivoAsync(dto.NombreUsuario);
        if (existente is not null)
            return (null, "Ya existe un usuario con ese nombre.");

        // Vínculo 1:1 con Asesor: obligatorio solo cuando Rol == "Asesor".
        if (dto.Rol == "Asesor")
        {
            if (!dto.AsesorId.HasValue)
                return (null, "Debe indicar el AsesorId cuando el rol es 'Asesor'.");

            var asesor = await this.asesorRepository.GetByIdAsync(dto.AsesorId.Value);
            if (asesor is null || !asesor.Activo)
                return (null, "El asesor indicado no existe o está inactivo.");

            // Validar que el asesor no tenga ya un usuario vinculado para evitar Error 500
            if (await this.usuarioRepository.ExisteAsesorVinculadoAsync(dto.AsesorId.Value))
                return (null, "Este asesor ya tiene una cuenta de usuario vinculada.");
        }
        else if (dto.AsesorId.HasValue)
        {
            return (null, "AsesorId solo aplica cuando el rol es 'Asesor'.");
        }

        var usuario = new Usuario
        {
            NombreUsuario = dto.NombreUsuario,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Rol = dto.Rol,
            AsesorId = dto.Rol == "Asesor" ? dto.AsesorId : null,
            Activo = true
        };

        await this.usuarioRepository.AddAsync(usuario);
        await this.usuarioRepository.SaveChangesAsync();

        return (MapToResponse(usuario), null);
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(int id, UpdateUsuarioDto dto)
    {
        var usuario = await this.usuarioRepository.GetByIdAsync(id);
        if (usuario is null || !usuario.Activo)
            return (false, "Usuario no encontrado.");

        if (!RolesValidos.Contains(dto.Rol))
            return (false, "El rol debe ser 'Administrador', 'Coordinador' o 'Asesor'.");

        if (dto.Rol == "Asesor")
        {
            if (!dto.AsesorId.HasValue)
                return (false, "Debe indicar el AsesorId cuando el rol es 'Asesor'.");

            var asesor = await this.asesorRepository.GetByIdAsync(dto.AsesorId.Value);
            if (asesor is null || !asesor.Activo)
                return (false, "El asesor indicado no existe o está inactivo.");

            // Validar que el asesor no tenga ya un usuario vinculado (que no sea el mismo usuario)
            if (await this.usuarioRepository.ExisteAsesorVinculadoAsync(dto.AsesorId.Value, id))
                return (false, "Este asesor ya tiene otra cuenta de usuario vinculada.");
        }
        else if (dto.AsesorId.HasValue)
        {
            return (false, "AsesorId solo aplica cuando el rol es 'Asesor'.");
        }

        usuario.Rol = dto.Rol;
        usuario.AsesorId = dto.Rol == "Asesor" ? dto.AsesorId : null;

        await this.usuarioRepository.UpdateAsync(usuario);
        await this.usuarioRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DesactivarAsync(int id)
    {
        var usuario = await this.usuarioRepository.GetByIdAsync(id);
        if (usuario is null || !usuario.Activo)
            return (false, "Usuario no encontrado.");

        usuario.Activo = false;

        await this.usuarioRepository.UpdateAsync(usuario);
        await this.usuarioRepository.SaveChangesAsync();

        return (true, null);
    }

    private static UsuarioResponseDto MapToResponse(Usuario u) => new()
    {
        Id = u.Id,
        NombreUsuario = u.NombreUsuario,
        Rol = u.Rol,
        AsesorId = u.AsesorId,
        Activo = u.Activo
    };
}