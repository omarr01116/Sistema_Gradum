using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Interfaces;

public interface IUsuarioRepository
{
    // Ya existía (Commit 4) — usado por AuthService.LoginAsync. No se toca.
    Task<Usuario?> GetByUsernameAsync(string nombreUsuario);

    // Nuevo: para detectar duplicados de nombre sin importar si está activo o no.
    Task<Usuario?> GetByUsernameIncludingInactivoAsync(string nombreUsuario);

    Task<List<Usuario>> GetAllAsync();
    Task<Usuario?> GetByIdAsync(int id);
    Task AddAsync(Usuario usuario);
    Task UpdateAsync(Usuario usuario);
    Task SaveChangesAsync();
}