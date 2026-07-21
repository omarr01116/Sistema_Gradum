using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Infrastructure.Data.Configurations;

public class ConfiguracionSistemaConfiguration : IEntityTypeConfiguration<ConfiguracionSistema>
{
    public void Configure(EntityTypeBuilder<ConfiguracionSistema> builder)
    {
        builder.ToTable("configuraciones_sistema");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Clave).HasMaxLength(100).IsRequired();
        builder.HasIndex(c => c.Clave).IsUnique();

        builder.Property(c => c.Valor).HasMaxLength(255).IsRequired();
        builder.Property(c => c.Descripcion).HasMaxLength(255);
    }
}