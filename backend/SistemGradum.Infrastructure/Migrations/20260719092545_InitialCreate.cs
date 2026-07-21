using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SistemGradum.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "alertas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProyectoId = table.Column<int>(type: "integer", nullable: true),
                    HitoId = table.Column<int>(type: "integer", nullable: true),
                    UsuarioDestinoId = table.Column<int>(type: "integer", nullable: false),
                    Mensaje = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FechaHora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Leida = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alertas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "asesores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombres = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Apellidos = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Especialidad = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asesores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoCliente = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Nombres = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Apellidos = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DniPasaporte = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    EstadoFinanciero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "configuraciones_sistema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Clave = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Valor = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configuraciones_sistema", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreUsuario = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Rol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AsesorId = table.Column<int>(type: "integer", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_usuarios_asesores_AsesorId",
                        column: x => x.AsesorId,
                        principalTable: "asesores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "proyectos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoProyecto = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    AsesorId = table.Column<int>(type: "integer", nullable: true),
                    Universidad = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Carrera = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Programa = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    TipoProyecto = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Tema = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaEntregaComprometida = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EstadoProyecto = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UsuarioUltimoCambioId = table.Column<int>(type: "integer", nullable: true),
                    FechaUltimoCambio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proyectos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_proyectos_asesores_AsesorId",
                        column: x => x.AsesorId,
                        principalTable: "asesores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_proyectos_clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "documentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Categoria = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NombreArchivoOriginal = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    VersionActual = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_documentos_proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "hitos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    NombreHito = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PesoPorcentual = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    FechaCompromiso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EstadoHito = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    UsuarioCompletadoId = table.Column<int>(type: "integer", nullable: true),
                    FechaCompletado = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioAprobadorId = table.Column<int>(type: "integer", nullable: true),
                    FechaAprobacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RazonRechazo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hitos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_hitos_proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "observaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Detalle = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_observaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_observaciones_proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "versiones_documento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentoId = table.Column<int>(type: "integer", nullable: false),
                    NumeroVersion = table.Column<int>(type: "integer", nullable: false),
                    RutaArchivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FechaSubida = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_versiones_documento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_versiones_documento_documentos_DocumentoId",
                        column: x => x.DocumentoId,
                        principalTable: "documentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_alertas_HitoId",
                table: "alertas",
                column: "HitoId");

            migrationBuilder.CreateIndex(
                name: "IX_alertas_ProyectoId",
                table: "alertas",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_alertas_UsuarioDestinoId",
                table: "alertas",
                column: "UsuarioDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_clientes_CodigoCliente",
                table: "clientes",
                column: "CodigoCliente",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_configuraciones_sistema_Clave",
                table: "configuraciones_sistema",
                column: "Clave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_documentos_ProyectoId",
                table: "documentos",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_hitos_ProyectoId",
                table: "hitos",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_hitos_UsuarioAprobadorId",
                table: "hitos",
                column: "UsuarioAprobadorId");

            migrationBuilder.CreateIndex(
                name: "IX_hitos_UsuarioCompletadoId",
                table: "hitos",
                column: "UsuarioCompletadoId");

            migrationBuilder.CreateIndex(
                name: "IX_observaciones_ProyectoId",
                table: "observaciones",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_observaciones_UsuarioId",
                table: "observaciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_proyectos_AsesorId",
                table: "proyectos",
                column: "AsesorId");

            migrationBuilder.CreateIndex(
                name: "IX_proyectos_ClienteId",
                table: "proyectos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_proyectos_CodigoProyecto",
                table: "proyectos",
                column: "CodigoProyecto",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_proyectos_UsuarioUltimoCambioId",
                table: "proyectos",
                column: "UsuarioUltimoCambioId");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_AsesorId",
                table: "usuarios",
                column: "AsesorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_NombreUsuario",
                table: "usuarios",
                column: "NombreUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_versiones_documento_DocumentoId_NumeroVersion",
                table: "versiones_documento",
                columns: new[] { "DocumentoId", "NumeroVersion" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_versiones_documento_UsuarioId",
                table: "versiones_documento",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alertas");

            migrationBuilder.DropTable(
                name: "configuraciones_sistema");

            migrationBuilder.DropTable(
                name: "hitos");

            migrationBuilder.DropTable(
                name: "observaciones");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "versiones_documento");

            migrationBuilder.DropTable(
                name: "documentos");

            migrationBuilder.DropTable(
                name: "proyectos");

            migrationBuilder.DropTable(
                name: "asesores");

            migrationBuilder.DropTable(
                name: "clientes");
        }
    }
}
