using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class NuevoPlanning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cuestionarios_Asignaciones_ContratoId",
                table: "Cuestionarios");

            migrationBuilder.DropTable(
                name: "Asignaciones");

            migrationBuilder.DropColumn(
                name: "Login",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "Credenciales_EmailVerificado",
                table: "Usuarios",
                newName: "EmailVerificado");

            migrationBuilder.RenameColumn(
                name: "Credenciales_Email",
                table: "Usuarios",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "Credenciales_CodigoVerificacionEmail",
                table: "Usuarios",
                newName: "CodigoVerificacionEmail");

            migrationBuilder.RenameColumn(
                name: "ContratoId",
                table: "Cuestionarios",
                newName: "AlumnoId");

            migrationBuilder.RenameIndex(
                name: "IX_Cuestionarios_ContratoId",
                table: "Cuestionarios",
                newName: "IX_Cuestionarios_AlumnoId");

            migrationBuilder.AddColumn<Guid>(
                name: "EntrenadorId",
                table: "Usuarios",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_EntrenadorId",
                table: "Usuarios",
                column: "EntrenadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cuestionarios_Usuarios_AlumnoId",
                table: "Cuestionarios",
                column: "AlumnoId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Usuarios_EntrenadorId",
                table: "Usuarios",
                column: "EntrenadorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cuestionarios_Usuarios_AlumnoId",
                table: "Cuestionarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Usuarios_EntrenadorId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_EntrenadorId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "EntrenadorId",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "EmailVerificado",
                table: "Usuarios",
                newName: "Credenciales_EmailVerificado");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Usuarios",
                newName: "Credenciales_Email");

            migrationBuilder.RenameColumn(
                name: "CodigoVerificacionEmail",
                table: "Usuarios",
                newName: "Credenciales_CodigoVerificacionEmail");

            migrationBuilder.RenameColumn(
                name: "AlumnoId",
                table: "Cuestionarios",
                newName: "ContratoId");

            migrationBuilder.RenameIndex(
                name: "IX_Cuestionarios_AlumnoId",
                table: "Cuestionarios",
                newName: "IX_Cuestionarios_ContratoId");

            migrationBuilder.AddColumn<string>(
                name: "Login",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Asignaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AlumnoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntrenadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaDesasignacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asignaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asignaciones_Usuarios_AlumnoId",
                        column: x => x.AlumnoId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Asignaciones_Usuarios_EntrenadorId",
                        column: x => x.EntrenadorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asignaciones_AlumnoId",
                table: "Asignaciones",
                column: "AlumnoId");

            migrationBuilder.CreateIndex(
                name: "IX_Asignaciones_EntrenadorId",
                table: "Asignaciones",
                column: "EntrenadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cuestionarios_Asignaciones_ContratoId",
                table: "Cuestionarios",
                column: "ContratoId",
                principalTable: "Asignaciones",
                principalColumn: "Id");
        }
    }
}
