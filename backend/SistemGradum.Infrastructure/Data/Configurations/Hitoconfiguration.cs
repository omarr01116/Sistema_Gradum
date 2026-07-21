using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Infrastructure.Data.Configurations;

public class HitoConfiguration : IEntityTypeConfiguration<Hito>
{
    public void Configure(EntityTypeBuilder<Hito> builder)
    {
        builder.ToTable("hitos");
        builder.HasKey(h => h.Id);

        builder.Property(h => h.NombreHito).HasMaxLength(200).IsRequired();
        builder.Property(h => h.PesoPorcentual).HasColumnType("decimal(5,2)");
        builder.Property(h => h.EstadoHito).HasMaxLength(30).IsRequired();
        builder.Property(h => h.RazonRechazo).HasMaxLength(500);

        // Hito es parte del agregado Proyecto: si se borra el proyecto,
        // se borran sus hitos (a diferencia de Cliente/Asesor, que se desactivan).
        builder.HasOne(h => h.Proyecto)
            .WithMany(p => p.Hitos)
            .HasForeignKey(h => h.ProyectoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Referencias débiles a Usuario, sin FK física (ver justificación).
        builder.HasIndex(h => h.UsuarioCompletadoId);
        builder.HasIndex(h => h.UsuarioAprobadorId);

        // Fecha de calendario pura: mismo fix aplicado en Proyecto (Commit 7)
        // para evitar el error de Npgsql con Kind=Unspecified.
        builder.Property(h => h.FechaCompromiso).HasColumnType("date");

        builder.HasOne(h => h.DocumentoEvidencia)
            .WithMany()
            .HasForeignKey(h => h.DocumentoEvidenciaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}