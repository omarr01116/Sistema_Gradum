using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemGradum.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ArreglarEsquemaDocumentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NombreArchivoOriginal",
                table: "documentos");

            migrationBuilder.DropColumn(
                name: "VersionActual",
                table: "documentos");

            migrationBuilder.AddColumn<string>(
                name: "NombreArchivoOriginal",
                table: "versiones_documento",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "documentos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NombreArchivoOriginal",
                table: "versiones_documento");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "documentos");

            migrationBuilder.AddColumn<string>(
                name: "NombreArchivoOriginal",
                table: "documentos",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "VersionActual",
                table: "documentos",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
