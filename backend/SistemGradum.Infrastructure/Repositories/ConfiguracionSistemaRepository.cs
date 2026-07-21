using Microsoft.EntityFrameworkCore;
using SistemGradum.Application.Interfaces;
using SistemGradum.Domain.Entities;
using SistemGradum.Infrastructure.Data;

namespace SistemGradum.Infrastructure.Repositories;

public class ConfiguracionSistemaRepository : IConfiguracionSistemaRepository
{
    private readonly SistemGradumDbContext context;

    public ConfiguracionSistemaRepository(SistemGradumDbContext context)
    {
        this.context = context;
    }

    // Se quitó AsNoTracking(): este método ahora también se usa para Update
    // (Commit 12), así que EF Core necesita poder rastrear los cambios.
    public async Task<ConfiguracionSistema?> GetByClaveAsync(string clave)
    {
        return await this.context.ConfiguracionesSistema
            .FirstOrDefaultAsync(c => c.Clave == clave);
    }

    public async Task AddAsync(ConfiguracionSistema configuracion)
    {
        await this.context.ConfiguracionesSistema.AddAsync(configuracion);
    }

    public async Task UpdateAsync(ConfiguracionSistema configuracion)
    {
        this.context.ConfiguracionesSistema.Update(configuracion);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await this.context.SaveChangesAsync();
    }
}