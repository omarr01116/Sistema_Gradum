using SistemGradum.Application.DTOs;
using SistemGradum.Application.Interfaces;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository clienteRepository;

    private static readonly string[] EstadosFinancierosValidos = { "AlDia", "ConDeuda" };

    public ClienteService(IClienteRepository clienteRepository)
    {
        this.clienteRepository = clienteRepository;
    }

    public async Task<List<ClienteResponseDto>> GetAllAsync()
    {
        var clientes = await this.clienteRepository.GetAllAsync();
        return clientes.Select(MapToResponse).ToList();
    }

    public async Task<ClienteResponseDto?> GetByIdAsync(int id)
    {
        var cliente = await this.clienteRepository.GetByIdAsync(id);
        return cliente is null || !cliente.Activo ? null : MapToResponse(cliente);
    }

    public async Task<(ClienteResponseDto? Cliente, string? Error)> CreateAsync(CreateClienteDto dto)
    {
        if (!EstadosFinancierosValidos.Contains(dto.EstadoFinanciero))
            return (null, "El estado financiero debe ser 'AlDia' o 'ConDeuda'.");

        var cliente = new Cliente
        {
            CodigoCliente = await GenerarCodigoAsync(),
            Nombres = dto.Nombres,
            Apellidos = dto.Apellidos,
            DniPasaporte = dto.DniPasaporte,
            Telefono = dto.Telefono,
            Email = dto.Email,
            EstadoFinanciero = dto.EstadoFinanciero,
            Activo = true
        };

        await this.clienteRepository.AddAsync(cliente);
        await this.clienteRepository.SaveChangesAsync();

        return (MapToResponse(cliente), null);
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(int id, UpdateClienteDto dto)
    {
        var cliente = await this.clienteRepository.GetByIdAsync(id);
        if (cliente is null || !cliente.Activo)
            return (false, "Cliente no encontrado.");

        if (!EstadosFinancierosValidos.Contains(dto.EstadoFinanciero))
            return (false, "El estado financiero debe ser 'AlDia' o 'ConDeuda'.");

        cliente.Nombres = dto.Nombres;
        cliente.Apellidos = dto.Apellidos;
        cliente.Telefono = dto.Telefono;
        cliente.Email = dto.Email;
        cliente.EstadoFinanciero = dto.EstadoFinanciero;

        await this.clienteRepository.UpdateAsync(cliente);
        await this.clienteRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var cliente = await this.clienteRepository.GetByIdAsync(id);
        if (cliente is null || !cliente.Activo)
            return (false, "Cliente no encontrado.");

        cliente.Activo = false;

        await this.clienteRepository.UpdateAsync(cliente);
        await this.clienteRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<bool> ReactivarAsync(int id)
    {
        var cliente = await this.clienteRepository.GetByIdAsync(id);

        if (cliente is null || cliente.Activo)
            return false;

        cliente.Activo = true;

        await this.clienteRepository.UpdateAsync(cliente);
        await this.clienteRepository.SaveChangesAsync();

        return true;
    }

    private async Task<string> GenerarCodigoAsync()
    {
        var siguiente = await this.clienteRepository.CountAsync() + 1;
        return $"CLI-{siguiente:D4}";
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