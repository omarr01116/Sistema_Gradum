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

    public async Task<ConfiguracionSistema?> GetByClaveAsync(string clave)
    {
        return await this.context.ConfiguracionesSistema
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Clave == clave);
    }
}