using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Infrastructure.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.NombreUsuario).HasMaxLength(50).IsRequired();
        builder.HasIndex(u => u.NombreUsuario).IsUnique();

        builder.Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();
        builder.Property(u => u.Rol).HasMaxLength(20).IsRequired();

        // Relación 1:1 opcional con Asesor. El índice único sobre AsesorId
        // es lo que garantiza que un Asesor no pueda tener dos cuentas de
        // Usuario (en MySQL, múltiples filas con AsesorId = NULL sí están
        // permitidas en un índice único, así que Admin/Coordinador no chocan).
        builder.HasIndex(u => u.AsesorId).IsUnique();
        builder.HasOne(u => u.Asesor)
            .WithOne(a => a.Usuario)
            .HasForeignKey<Usuario>(u => u.AsesorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}