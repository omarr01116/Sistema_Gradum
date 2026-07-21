using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Infrastructure.Data.Configurations;

public class DocumentoConfiguration : IEntityTypeConfiguration<Documento>
{
    public void Configure(EntityTypeBuilder<Documento> builder)
    {
        builder.ToTable("documentos");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Categoria).HasMaxLength(50).IsRequired();
        builder.Property(d => d.NombreArchivoOriginal).HasMaxLength(255).IsRequired();

        builder.HasOne(d => d.Proyecto)
            .WithMany(p => p.Documentos)
            .HasForeignKey(d => d.ProyectoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}