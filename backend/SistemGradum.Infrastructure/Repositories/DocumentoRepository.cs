using Microsoft.EntityFrameworkCore;
using SistemGradum.Application.Interfaces;
using SistemGradum.Domain.Entities;
using SistemGradum.Infrastructure.Data;

namespace SistemGradum.Infrastructure.Repositories;

public class DocumentoRepository : IDocumentoRepository
{
    private readonly SistemGradumDbContext context;

    public DocumentoRepository(SistemGradumDbContext context)
    {
        this.context = context;
    }

    public async Task<List<Documento>> GetByProyectoIdAsync(int proyectoId)
    {
        return await this.context.Documentos
            .Include(d => d.Versiones)
            .Where(d => d.ProyectoId == proyectoId)
            .ToListAsync();
    }

    public async Task<Documento?> GetByProyectoIdYCategoriaAsync(int proyectoId, string categoria)
    {
        return await this.context.Documentos
            .Include(d => d.Versiones)
            .FirstOrDefaultAsync(d => d.ProyectoId == proyectoId && d.Categoria == categoria);
    }

    public async Task<Documento?> GetByIdConVersionesAsync(int id)
    {
        return await this.context.Documentos
            .Include(d => d.Versiones)
            .Include(d => d.Proyecto)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task AddDocumentoAsync(Documento documento) =>
        await this.context.Documentos.AddAsync(documento);

    public async Task AddVersionAsync(VersionDocumento version) =>
        await this.context.VersionesDocumento.AddAsync(version);

    public async Task SaveChangesAsync() => await this.context.SaveChangesAsync();
}