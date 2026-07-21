using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Interfaces;
public interface IUsuarioRepository
{
      Task<Usuario?> GetByUsernameAsync(string nombreUsuario);
}