using Microsoft.EntityFrameworkCore;
using SistemGradum.Application.Interfaces;
using SistemGradum.Domain.Entities;
using SistemGradum.Infrastructure.Data;

namespace SistemGradum.Infrastructure.Repositories;

public class AsesorRepository : IAsesorRepository
{
    private readonly SistemGradumDbContext _context;

    public AsesorRepository(SistemGradumDbContext context)
    {
        _context = context;
    }

    public async Task<List<Asesor>> GetAllAsync()
    {
        return await _context.Asesores
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Asesor?> GetByIdAsync(int id)
    {
        return await _context.Asesores
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task AddAsync(Asesor asesor)
    {
        await _context.Asesores.AddAsync(asesor);
    }

    public async Task UpdateAsync(Asesor asesor)
    {
        _context.Asesores.Update(asesor);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}