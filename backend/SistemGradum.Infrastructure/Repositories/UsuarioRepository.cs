using Microsoft.EntityFrameworkCore;
using SistemGradum.Application.Interfaces;
using SistemGradum.Domain.Entities;
using SistemGradum.Infrastructure.Data;

namespace SistemGradum.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly SistemGradumDbContext context;

    public UsuarioRepository(SistemGradumDbContext context)
    {
        this.context = context;
    }

    public async Task<Usuario?> GetByUsernameAsync(string nombreUsuario)
    {
        return await this.context.Usuarios
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Activo);
    }
}