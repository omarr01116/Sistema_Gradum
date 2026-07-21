using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Interfaces;

public interface IDocumentoRepository
{
    Task<List<Documento>> GetByProyectoIdAsync(int proyectoId);
    Task<Documento?> GetByProyectoIdYCategoriaAsync(int proyectoId, string categoria);
    Task<Documento?> GetByIdConVersionesAsync(int id);
    Task AddDocumentoAsync(Documento documento);
    Task AddVersionAsync(VersionDocumento version);
    Task SaveChangesAsync();
}