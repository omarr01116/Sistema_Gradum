using Microsoft.EntityFrameworkCore;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Infrastructure.Data;

public class SistemGradumDbContext : DbContext
{
    public SistemGradumDbContext(DbContextOptions<SistemGradumDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Asesor> Asesores => Set<Asesor>();
    public DbSet<Proyecto> Proyectos => Set<Proyecto>();
    public DbSet<Hito> Hitos => Set<Hito>();
    public DbSet<Observacion> Observaciones => Set<Observacion>();
    public DbSet<Documento> Documentos => Set<Documento>();
    public DbSet<VersionDocumento> VersionesDocumento => Set<VersionDocumento>();
    public DbSet<Alerta> Alertas => Set<Alerta>();
    public DbSet<ConfiguracionSistema> ConfiguracionesSistema => Set<ConfiguracionSistema>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplica automáticamente todas las clases IEntityTypeConfiguration<T>
        // del ensamblado (carpeta Data/Configurations/), sin tener que
        // registrarlas una por una aquí.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SistemGradumDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}