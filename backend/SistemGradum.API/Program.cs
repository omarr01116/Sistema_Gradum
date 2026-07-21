using SistemGradum.Application.Interfaces;
using SistemGradum.Application.Services;
using SistemGradum.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using SistemGradum.Infrastructure.Data;
// Asegúrate de incluir los namespaces correctos donde tengas AuthService y TokenService

var builder = WebApplication.CreateBuilder(args);

// 1. Habilitar el uso de controladores
builder.Services.AddControllers(); 

// 2. Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Inyección de dependencias (Registrar tus servicios)
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>(); 
var connectionString = builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<SistemGradumDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36))));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Habilitar autorización (importante ya que manejarás autenticación y roles)
app.UseAuthorization();

// 4. Mapear los endpoints de los controladores para que Swagger y la API los reconozcan
app.MapControllers();

app.Run();