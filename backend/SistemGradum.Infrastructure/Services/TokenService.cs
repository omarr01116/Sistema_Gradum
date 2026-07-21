using SistemGradum.Application.Interfaces;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Infrastructure.Services;

public class TokenService : ITokenService
{
    public string GenerarToken(Usuario usuario)
    {
        return $"{usuario.Id}-{usuario.NombreUsuario}-{usuario.Rol}-{Guid.NewGuid()}";
    }
}