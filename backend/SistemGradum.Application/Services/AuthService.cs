using SistemGradum.Application.DTOs;
using SistemGradum.Application.Interfaces;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Services;

// Version 1: usuarios "sembrados" en memoria, contraseña en texto plano.
// Pendiente para versiones futuras:
//   - Hashear contraseñas (BCrypt) en vez de compararlas en texto plano.
//   - Reemplazar la lista en memoria por una consulta real a MySQL con EF Core.
public class AuthService : IAuthService
{
    private readonly ITokenService tokenService;

    private readonly List<Usuario> usuarios = new()
    {
        new Usuario
        {
            Id = 1,
            NombreUsuario = "admin",
            PasswordHash = "Admin123!", // temporal: texto plano, se hasheará luego
            Rol = "Administrador"
        },
        new Usuario
        {
            Id = 2,
            NombreUsuario = "coordinador",
            PasswordHash = "Coord123!", // temporal: texto plano, se hasheará luego
            Rol = "Coordinador"
        }
    };

    public AuthService(ITokenService tokenService)
    {
        this.tokenService = tokenService;
    }

    public LoginResponseDto? IniciarSesion(LoginRequestDto dto)
    {
        var usuario = this.usuarios.FirstOrDefault(u => u.NombreUsuario == dto.NombreUsuario && u.Activo);
        if (usuario is null) return null;

        var credencialesValidas = usuario.PasswordHash == dto.Password;
        if (!credencialesValidas) return null;

        var token = this.tokenService.GenerarToken(usuario);

        return new LoginResponseDto
        {
            Token = token,
            NombreUsuario = usuario.NombreUsuario,
            Rol = usuario.Rol
        };
    }
}