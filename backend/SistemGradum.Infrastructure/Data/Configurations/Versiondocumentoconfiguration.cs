using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Infrastructure.Data.Configurations;

public class VersionDocumentoConfiguration : IEntityTypeConfiguration<VersionDocumento>
{
    public void Configure(EntityTypeBuilder<VersionDocumento> builder)
    {
        builder.ToTable("versiones_documento");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.RutaArchivo).HasMaxLength(500).IsRequired();

        // RN-07: numeración secuencial única por documento, reforzada a nivel de esquema.
        builder.HasIndex(v => new { v.DocumentoId, v.NumeroVersion }).IsUnique();

        builder.HasOne(v => v.Documento)
            .WithMany(d => d.Versiones)
            .HasForeignKey(v => v.DocumentoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Referencia débil a Usuario, sin FK física (ver justificación).
        builder.HasIndex(v => v.UsuarioId);
    }
}