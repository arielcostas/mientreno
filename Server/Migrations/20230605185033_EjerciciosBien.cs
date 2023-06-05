using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mientreno.Server.Migrations
{
    /// <inheritdoc />
    public partial class EjerciciosBien : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JornadaEntrenamiento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClienteAsignadoId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaRealizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Valoracion = table.Column<int>(type: "int", nullable: true),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JornadaEntrenamiento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JornadaEntrenamiento_AspNetUsers_ClienteAsignadoId",
                        column: x => x.ClienteAsignadoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EjercicioProgramado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JornadaId = table.Column<int>(type: "int", nullable: false),
                    EjercicioId = table.Column<int>(type: "int", nullable: false),
                    Series = table.Column<int>(type: "int", nullable: false),
                    Repeticiones = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EjercicioProgramado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EjercicioProgramado_Ejercicios_EjercicioId",
                        column: x => x.EjercicioId,
                        principalTable: "Ejercicios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EjercicioProgramado_JornadaEntrenamiento_JornadaId",
                        column: x => x.JornadaId,
                        principalTable: "JornadaEntrenamiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EjercicioProgramado_EjercicioId",
                table: "EjercicioProgramado",
                column: "EjercicioId");

            migrationBuilder.CreateIndex(
                name: "IX_EjercicioProgramado_JornadaId",
                table: "EjercicioProgramado",
                column: "JornadaId");

            migrationBuilder.CreateIndex(
                name: "IX_JornadaEntrenamiento_ClienteAsignadoId",
                table: "JornadaEntrenamiento",
                column: "ClienteAsignadoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EjercicioProgramado");

            migrationBuilder.DropTable(
                name: "JornadaEntrenamiento");
        }
    }
}
