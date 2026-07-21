using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Interfaces;

public interface IConfiguracionSistemaRepository
{
    Task<ConfiguracionSistema?> GetByClaveAsync(string clave);
}