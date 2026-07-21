using SistemGradum.Application.DTOs;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Interfaces;

public interface IClienteService
{
    Task<Cliente> Crear(CreateClienteDto dto);
    Task<List<Cliente>> ObtenerTodos();
}