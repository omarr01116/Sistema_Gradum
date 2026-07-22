# Despliegue del Backend de GRADUM en Azure

Este documento detalla los pasos y variables de entorno necesarias para desplegar exitosamente el backend en Azure App Service.

## Requisitos Previos

- **Azure App Service (Linux)** para alojar la API.
- **Azure Database for PostgreSQL Flexible Server** para la base de datos.
- **Azure Storage Account** (Blob Storage) para el almacenamiento de archivos de evidencia.
- **Azure Container Registry (ACR)** o Docker Hub si decides desplegar usando contenedores, o GitHub Actions para despliegue directo desde código.

## Variables de Entorno (Configuration Settings)

Para que el sistema funcione correctamente en producción, debes configurar las siguientes variables de entorno en tu App Service (en la sección *Settings > Configuration* o *Environment variables*):

| Variable | Descripción | Ejemplo |
|----------|-------------|---------|
| `ConnectionStrings__DefaultConnection` | Cadena de conexión a PostgreSQL. | `Host=miservidor.postgres.database.azure.com;Database=sistemgradum_db;Username=miusuario;Password=mipassword;` |
| `Jwt__Key` | Llave secreta para firmar los tokens JWT (debe ser larga y segura). | `TuLlaveSecretaSuperLargaYComplejaParaProduccion` |
| `AzureBlobStorage__ConnectionString` | Cadena de conexión de la cuenta de almacenamiento de Azure donde se guardarán los archivos. | `DefaultEndpointsProtocol=https;AccountName=micuenta;AccountKey=miclave;EndpointSuffix=core.windows.net` |
| `AzureBlobStorage__ContainerName` | Nombre del contenedor (si no existe, el sistema intentará crearlo). | `evidencias` |
| `Cors__AllowedOrigin` | URL del frontend desplegado para permitir peticiones CORS. | `https://mifrontend-gradum.azurewebsites.net` |

> **Nota:** En local, el sistema funcionará almacenando en disco local y usando la base de datos definida en `appsettings.Development.json`, a menos que se configure `AzureBlobStorage__ConnectionString`, en cuyo caso usará Azure Blob Storage también en local.

## Uso con Docker (Opcional)

Si optas por el despliegue con contenedores (Web App for Containers):

1. **Construye la imagen desde la carpeta `backend`** (donde está la solución `.sln` y los proyectos):
   ```bash
   docker build -t sistemgradum-api:latest -f SistemGradum.API/Dockerfile .
   ```
2. **Sube la imagen a tu Container Registry** y configúrala en el App Service.
3. El contenedor expone el puerto **8080**, el cual es automáticamente mapeado por App Service en Linux.

## Consideraciones Adicionales

- **Migraciones EF Core:** Asegúrate de aplicar las migraciones a la base de datos de producción antes del primer uso, ya sea ejecutando `dotnet ef database update` desde un entorno con acceso a la base de datos, o mediante un pipeline CI/CD.
- **Timezone:** La base de datos guarda las fechas en UTC. Si es necesario procesarlas en zona horaria local, el frontend debe realizar la conversión correspondiente, o se debe configurar la zona horaria a nivel aplicación.
