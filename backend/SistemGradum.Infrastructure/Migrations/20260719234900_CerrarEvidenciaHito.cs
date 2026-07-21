using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemGradum.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CerrarEvidenciaHito : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_documentos_ProyectoId",
                table: "documentos");

            migrationBuilder.DropColumn(
                name: "RutaEvidenciaTemporal",
                table: "hitos");

            migrationBuilder.AddColumn<int>(
                name: "DocumentoEvidenciaId",
                table: "hitos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_hitos_DocumentoEvidenciaId",
                table: "hitos",
                column: "DocumentoEvidenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_documentos_ProyectoId_Categoria",
                table: "documentos",
                columns: new[] { "ProyectoId", "Categoria" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_hitos_documentos_DocumentoEvidenciaId",
                table: "hitos",
                column: "DocumentoEvidenciaId",
                principalTable: "documentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_hitos_documentos_DocumentoEvidenciaId",
                table: "hitos");

            migrationBuilder.DropIndex(
                name: "IX_hitos_DocumentoEvidenciaId",
                table: "hitos");

            migrationBuilder.DropIndex(
                name: "IX_documentos_ProyectoId_Categoria",
                table: "documentos");

            migrationBuilder.DropColumn(
                name: "DocumentoEvidenciaId",
                table: "hitos");

            migrationBuilder.AddColumn<string>(
                name: "RutaEvidenciaTemporal",
                table: "hitos",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_documentos_ProyectoId",
                table: "documentos",
                column: "ProyectoId");
        }
    }
}
