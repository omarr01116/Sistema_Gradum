using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SistemGradum.Application.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SistemGradum.Infrastructure.Storage;

public class AlmacenamientoBlobAzure : IAlmacenamientoArchivos
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public AlmacenamientoBlobAzure(IConfiguration config)
    {
        var connectionString = config["AzureBlobStorage:ConnectionString"]
            ?? throw new ArgumentNullException("AzureBlobStorage:ConnectionString", "La cadena de conexión de Azure Blob Storage no está configurada.");

        this._containerName = config["AzureBlobStorage:ContainerName"] ?? "evidencias";
        this._blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<string> GuardarAsync(int proyectoId, string categoria, IFormFile archivo)
    {
        var containerClient = this._blobServiceClient.GetBlobContainerClient(this._containerName);
        await containerClient.CreateIfNotExistsAsync();

        var categoriaSegura = this.SanitizarSegmento(categoria);
        var nombreOriginal = Path.GetFileName(archivo.FileName);
        var extension = Path.GetExtension(nombreOriginal);
        var nombreFisico = $"{Guid.NewGuid():N}{extension}";

        // Estructura virtual: {proyectoId}/{categoriaSegura}/{nombreFisico}
        var blobPath = $"{proyectoId}/{categoriaSegura}/{nombreFisico}";
        var blobClient = containerClient.GetBlobClient(blobPath);

        // Subida en modo streaming
        using (var stream = archivo.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, overwrite: true);
        }

        return blobPath;
    }

    public string ObtenerRutaCompleta(string rutaRelativa)
    {
        var containerClient = this._blobServiceClient.GetBlobContainerClient(this._containerName);
        var blobClient = containerClient.GetBlobClient(rutaRelativa);
        return blobClient.Uri.ToString();
    }

    public async Task<Stream> ObtenerStreamAsync(string rutaRelativa)
    {
        var containerClient = this._blobServiceClient.GetBlobContainerClient(this._containerName);
        var blobClient = containerClient.GetBlobClient(rutaRelativa);
        if (!await blobClient.ExistsAsync())
        {
            throw new FileNotFoundException("El archivo no existe en Azure Blob Storage.", rutaRelativa);
        }
        var downloadInfo = await blobClient.DownloadStreamingAsync();
        return downloadInfo.Value.Content;
    }

    public async Task EliminarAsync(string rutaRelativa)
    {
        var containerClient = this._blobServiceClient.GetBlobContainerClient(this._containerName);
        var blobClient = containerClient.GetBlobClient(rutaRelativa);
        await blobClient.DeleteIfExistsAsync();
    }

    private string SanitizarSegmento(string valor)
    {
        var invalidos = Path.GetInvalidFileNameChars();
        var limpio = new string(valor.Where(c => !invalidos.Contains(c)).ToArray());
        return string.IsNullOrWhiteSpace(limpio) ? "General" : limpio;
    }
}
