using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Infrastructure.Data.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("clientes");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.CodigoCliente).HasMaxLength(20).IsRequired();
        builder.HasIndex(c => c.CodigoCliente).IsUnique();

        builder.Property(c => c.Nombres).HasMaxLength(150).IsRequired();
        builder.Property(c => c.Apellidos).HasMaxLength(150).IsRequired();
        builder.Property(c => c.DniPasaporte).HasMaxLength(20).IsRequired();
        builder.Property(c => c.Telefono).HasMaxLength(20);
        builder.Property(c => c.Email).HasMaxLength(150);
        builder.Property(c => c.EstadoFinanciero).HasMaxLength(20).IsRequired();
    }
}