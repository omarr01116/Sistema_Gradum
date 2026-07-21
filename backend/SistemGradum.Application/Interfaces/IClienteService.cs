using SistemGradum.Application.DTOs;
using SistemGradum.domain.Entities;
using SistemGradum.Domain.Entities;

namespace SistemGradum.Application.Interfaces;

public interface IClienteService
{
    IEnumerable<Cliente> ObtenerTodos();
    Cliente? ObtenerPorId(int id);
    Cliente Crear(CreateClienteDto dto);
}