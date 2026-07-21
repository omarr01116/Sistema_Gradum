using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Infrastructure.Data.Configurations;

public class ObservacionConfiguration : IEntityTypeConfiguration<Observacion>
{
    public void Configure(EntityTypeBuilder<Observacion> builder)
    {
        builder.ToTable("observaciones");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Detalle).HasMaxLength(1000).IsRequired();

        builder.HasOne(o => o.Proyecto)
            .WithMany(p => p.Observaciones)
            .HasForeignKey(o => o.ProyectoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Referencia débil a Usuario, sin FK física (ver justificación).
        builder.HasIndex(o => o.UsuarioId);
    }
}