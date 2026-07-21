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

    public async Task<Usuario?> GetByUsernameIncludingInactivoAsync(string nombreUsuario)
    {
        return await this.context.Usuarios
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);
    }

    public async Task<List<Usuario>> GetAllAsync()
    {
        return await this.context.Usuarios
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Usuario?> GetByIdAsync(int id)
    {
        return await this.context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> ExisteAsesorVinculadoAsync(int asesorId, int? usuarioIdExcluido = null)
    {
        var query = this.context.Usuarios.Where(u => u.AsesorId == asesorId);
        if (usuarioIdExcluido.HasValue)
        {
            query = query.Where(u => u.Id != usuarioIdExcluido.Value);
        }
        return await query.AnyAsync();
    }

    public async Task AddAsync(Usuario usuario)
    {
        await this.context.Usuarios.AddAsync(usuario);
    }

    public async Task UpdateAsync(Usuario usuario)
    {
        this.context.Usuarios.Update(usuario);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await this.context.SaveChangesAsync();
    }
}