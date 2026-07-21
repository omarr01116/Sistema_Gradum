using Microsoft.AspNetCore.Http;

namespace SistemGradum.Application.Interfaces;

public interface IAlmacenamientoArchivos
{
    Task<string> GuardarAsync(int proyectoId, string categoria, IFormFile archivo);
    string ObtenerRutaCompleta(string rutaRelativa);
}