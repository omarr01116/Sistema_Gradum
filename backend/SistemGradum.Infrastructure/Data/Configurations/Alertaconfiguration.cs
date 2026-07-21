using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Infrastructure.Data.Configurations;

public class AlertaConfiguration : IEntityTypeConfiguration<Alerta>
{
    public void Configure(EntityTypeBuilder<Alerta> builder)
    {
        builder.ToTable("alertas");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Tipo).HasMaxLength(50).IsRequired();
        builder.Property(a => a.Mensaje).HasMaxLength(500).IsRequired();

        // Sin FK física hacia Proyecto/Hito/Usuario (asociación opcional
        // según el tipo de alerta, ver sección 10.1 del documento).
        // Se indexan porque RF-022 consulta alertas por usuario destino
        // en cada login.
        builder.HasIndex(a => a.UsuarioDestinoId);
        builder.HasIndex(a => new { a.UsuarioDestinoId, a.Leida });
        builder.HasIndex(a => a.ProyectoId);
        builder.HasIndex(a => a.HitoId);
    }
}