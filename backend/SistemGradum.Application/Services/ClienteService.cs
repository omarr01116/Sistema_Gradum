using SistemGradum.Application.DTOs;
using SistemGradum.Application.Interfaces;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Services;

public class ClienteService : IClienteService
{
    private readonly List<Cliente> clientes = new();
    private int siguienteId = 1;

    public Task<List<Cliente>> ObtenerTodos()
    {
        return Task.FromResult(clientes);
    }

    public Cliente? ObtenerPorId(int id)
    {
        return clientes.FirstOrDefault(c => c.Id == id);
    }

    public Task<Cliente> Crear(CreateClienteDto dto)
    {
        var cliente = new Cliente
        {
            Id = siguienteId,
            CodigoCliente = $"CLI-{siguienteId:D4}",
            Nombres = dto.Nombres,
            Apellidos = dto.Apellidos,
            DniPasaporte = dto.DniPasaporte,
            Telefono = dto.Telefono,
            Email = dto.Email,
            EstadoFinanciero = "Al día",
            Activo = true
        };

        siguienteId++;
        clientes.Add(cliente);

        return Task.FromResult(cliente);
    }
}