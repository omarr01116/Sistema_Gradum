using SistemGradum.Application.DTOs;
using SistemGradum.Application.Interfaces;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<List<ClienteResponseDto>> GetAllAsync()
    {
        var clientes = await _clienteRepository.GetAllAsync();
        return clientes.Select(MapToResponse).ToList();
    }

    public async Task<ClienteResponseDto?> GetByIdAsync(int id)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        return cliente is null || !cliente.Activo ? null : MapToResponse(cliente);
    }

    public async Task<ClienteResponseDto> CreateAsync(CreateClienteDto dto)
    {
        var cliente = new Cliente
        {
            CodigoCliente = await GenerarCodigoAsync(), // RF-002: código autogenerado
            Nombres = dto.Nombres,
            Apellidos = dto.Apellidos,
            DniPasaporte = dto.DniPasaporte,
            Telefono = dto.Telefono,
            Email = dto.Email,
            EstadoFinanciero = dto.EstadoFinanciero,
            Activo = true
        };

        await _clienteRepository.AddAsync(cliente);
        await _clienteRepository.SaveChangesAsync();

        return MapToResponse(cliente);
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(int id, UpdateClienteDto dto)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        if (cliente is null || !cliente.Activo)
            return (false, "Cliente no encontrado.");

        cliente.Nombres = dto.Nombres;
        cliente.Apellidos = dto.Apellidos;
        cliente.Telefono = dto.Telefono;
        cliente.Email = dto.Email;
        cliente.EstadoFinanciero = dto.EstadoFinanciero;

        await _clienteRepository.UpdateAsync(cliente);
        await _clienteRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        // RF-003: "desactivar clientes, manteniendo su historial de proyectos intacto"
        var cliente = await _clienteRepository.GetByIdAsync(id);
        if (cliente is null || !cliente.Activo)
            return (false, "Cliente no encontrado.");

        cliente.Activo = false;

        await _clienteRepository.UpdateAsync(cliente);
        await _clienteRepository.SaveChangesAsync();

        return (true, null);
    }

    private async Task<string> GenerarCodigoAsync()
    {
        var siguiente = await _clienteRepository.CountAsync() + 1;
        return $"CLI-{siguiente:D4}"; // CLI-0001, CLI-0002, ...
    }

    private static ClienteResponseDto MapToResponse(Cliente c) => new()
    {
        Id = c.Id,
        CodigoCliente = c.CodigoCliente,
        Nombres = c.Nombres,
        Apellidos = c.Apellidos,
        DniPasaporte = c.DniPasaporte,
        Telefono = c.Telefono,
        Email = c.Email,
        EstadoFinanciero = c.EstadoFinanciero,
        Activo = c.Activo
    };
}