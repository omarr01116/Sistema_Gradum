using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Interfaces;

public interface IConfiguracionSistemaRepository
{
    Task<ConfiguracionSistema?> GetByClaveAsync(string clave);
    Task AddAsync(ConfiguracionSistema configuracion);
    Task UpdateAsync(ConfiguracionSistema configuracion);   // ← nuevo
    Task SaveChangesAsync();  
}