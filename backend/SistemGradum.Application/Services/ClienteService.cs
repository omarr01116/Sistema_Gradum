using SistemGradum.Application.DTOs;
using SistemGradum.Application.Interfaces;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Services;

public class ClienteService : IClienteService
{
    private readonly List<Cliente> clientes = new();
    private int siguienteId = 1;

    public IEnumerable<Cliente> ObtenerTodos() => this.clientes;

    public Cliente? ObtenerPorId(int id) =>
        this.clientes.FirstOrDefault(c => c.Id == id);

    public Cliente Crear(CreateClienteDto dto)
    {
        var cliente = new Cliente
        {
            Id = this.siguienteId++,
            CodigoCliente = $"CLI-{this.siguienteId:D4}",
            Nombres = dto.Nombres,
            Apellidos = dto.Apellidos,
            DniPasaporte = dto.DniPasaporte,
            Telefono = dto.Telefono,
            Email = dto.Email,
            EstadoFinanciero = "Al día",
            Activo = true
        };

        this.clientes.Add(cliente);
        return cliente;
    }
}