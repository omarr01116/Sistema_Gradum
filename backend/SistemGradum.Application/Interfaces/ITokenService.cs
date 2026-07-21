using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Interfaces;

public interface ITokenService
{
    string GenerarToken(Usuario usuario);
}