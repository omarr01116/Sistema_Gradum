using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SistemGradum.Application.Interfaces;

namespace SistemGradum.Infrastructure.Storage;

public class AlmacenamientoArchivos : IAlmacenamientoArchivos
{
    private readonly string carpetaBase;

    public AlmacenamientoArchivos(IConfiguration config, IWebHostEnvironment env)
    {
        var carpetaConfigurada = config["Almacenamiento:CarpetaBase"] ?? "uploads";
        this.carpetaBase = Path.Combine(env.ContentRootPath, carpetaConfigurada);
    }

    public async Task<string> GuardarAsync(int proyectoId, string categoria, IFormFile archivo)
    {
        var categoriaSegura = this.SanitizarSegmento(categoria);
        var carpetaProyecto = Path.Combine(this.carpetaBase, proyectoId.ToString(), categoriaSegura);
        Directory.CreateDirectory(carpetaProyecto);

        var nombreOriginal = Path.GetFileName(archivo.FileName);
        var extension = Path.GetExtension(nombreOriginal);
        var nombreFisico = $"{Guid.NewGuid():N}{extension}";

        var rutaCompleta = Path.Combine(carpetaProyecto, nombreFisico);

        await using var stream = new FileStream(rutaCompleta, FileMode.Create);
        await archivo.CopyToAsync(stream);

        return Path.Combine(proyectoId.ToString(), categoriaSegura, nombreFisico);
    }

    public string ObtenerRutaCompleta(string rutaRelativa) =>
        Path.Combine(this.carpetaBase, rutaRelativa);

    private string SanitizarSegmento(string valor)
    {
        var invalidos = Path.GetInvalidFileNameChars();
        var limpio = new string(valor.Where(c => !invalidos.Contains(c)).ToArray());
        return string.IsNullOrWhiteSpace(limpio) ? "General" : limpio;
    }
}
