using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Infrastructure.Data.Configurations;

public class ProyectoConfiguration : IEntityTypeConfiguration<Proyecto>
{
    public void Configure(EntityTypeBuilder<Proyecto> builder)
    {
        builder.ToTable("proyectos");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.CodigoProyecto).HasMaxLength(20).IsRequired();
        builder.HasIndex(p => p.CodigoProyecto).IsUnique();

        builder.Property(p => p.Universidad).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Carrera).HasMaxLength(150).IsRequired();
        builder.Property(p => p.Programa).HasMaxLength(150);
        builder.Property(p => p.TipoProyecto).HasMaxLength(30).IsRequired();
        builder.Property(p => p.Tema).HasMaxLength(300).IsRequired();
        builder.Property(p => p.EstadoProyecto).HasMaxLength(20).IsRequired();

        // RN-01: obligatorio, un proyecto siempre pertenece a un cliente.
        // Restrict: Cliente se desactiva (campo Activo), no se borra físicamente,
        // así que no necesitamos (ni queremos) cascada aquí.
        builder.HasOne(p => p.Cliente)
            .WithMany(c => c.Proyectos)
            .HasForeignKey(p => p.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Opcional: un proyecto puede no tener asesor asignado todavía (RF-005/RF-006).
        builder.HasOne(p => p.Asesor)
            .WithMany(a => a.Proyectos)
            .HasForeignKey(p => p.AsesorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Referencia débil, sin FK física (ver justificación en la respuesta).
        builder.HasIndex(p => p.UsuarioUltimoCambioId);
    }
}