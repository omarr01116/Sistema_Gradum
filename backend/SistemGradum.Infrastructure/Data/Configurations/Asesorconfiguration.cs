using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Infrastructure.Data.Configurations;

public class AsesorConfiguration : IEntityTypeConfiguration<Asesor>
{
    public void Configure(EntityTypeBuilder<Asesor> builder)
    {
        builder.ToTable("asesores");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Nombres).HasMaxLength(150).IsRequired();
        builder.Property(a => a.Apellidos).HasMaxLength(150).IsRequired();
        builder.Property(a => a.Telefono).HasMaxLength(20);
        builder.Property(a => a.Email).HasMaxLength(150);
        builder.Property(a => a.Especialidad).HasMaxLength(150);
    }
}