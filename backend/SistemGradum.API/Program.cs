using SistemGradum.Application.Interfaces;
using SistemGradum.Application.Services;
using SistemGradum.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using SistemGradum.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SistemGradum.Infrastructure.Repositories;
using SistemGradum.Infrastructure.Storage;
using SistemGradum.Domain.Entities;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// 1. Habilitar el uso de controladores
builder.Services.AddControllers();

// 2. Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Ingresa el token JWT (sin la palabra 'Bearer', Swagger la agrega sola)"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var allowedOrigin = builder.Configuration["Cors:AllowedOrigin"] ?? "http://localhost:5173";
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy.WithOrigins(allowedOrigin)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 3. DbContext — PostgreSQL (Npgsql)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? builder.Configuration.GetConnectionString("Default"); // Mantener compatibilidad temporal
builder.Services.AddDbContext<SistemGradumDbContext>(options =>
    options.UseNpgsql(connectionString));

// 4. Autenticación JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

// 5. Inyección de dependencias
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IAsesorRepository, AsesorRepository>();
builder.Services.AddScoped<IAsesorService, AsesorService>();
builder.Services.AddScoped<IConfiguracionSistemaRepository, ConfiguracionSistemaRepository>();
builder.Services.AddScoped<IProyectoRepository, ProyectoRepository>();
builder.Services.AddScoped<IProyectoService, ProyectoService>();
builder.Services.AddScoped<IHitoRepository, HitoRepository>();
builder.Services.AddScoped<IHitoService, HitoService>();
builder.Services.AddScoped<IDocumentoRepository, DocumentoRepository>();
builder.Services.AddScoped<IDocumentoService, DocumentoService>();
var blobConnectionString = builder.Configuration["AzureBlobStorage:ConnectionString"];
if (!string.IsNullOrEmpty(blobConnectionString))
{
    builder.Services.AddSingleton<IAlmacenamientoArchivos, AlmacenamientoBlobAzure>();
}
else
{
    builder.Services.AddSingleton<IAlmacenamientoArchivos, AlmacenamientoArchivos>();
}
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IObservacionRepository, ObservacionRepository>();
builder.Services.AddScoped<IObservacionService, ObservacionService>();
builder.Services.AddScoped<IAlertaRepository, AlertaRepository>();
builder.Services.AddScoped<IAlertaService, AlertaService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;
        var env = context.RequestServices.GetRequiredService<IWebHostEnvironment>();

        var responseObj = new
        {
            mensaje = exception is UnauthorizedAccessException ? exception.Message : "Ocurrió un error interno en el servidor.",
            detalle = env.IsDevelopment() ? exception?.Message : null,
            trace = env.IsDevelopment() ? exception?.StackTrace : null
        };

        if (exception is UnauthorizedAccessException)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }

        var json = JsonSerializer.Serialize(responseObj);
        await context.Response.WriteAsync(json);
    });
});

app.UseHttpsRedirection();

app.UseCors("FrontendDev");

app.UseAuthentication(); // primero autentica
app.UseAuthorization();  // luego autoriza — el orden importa

app.MapControllers();

app.Run();