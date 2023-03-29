using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class First : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Login = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaEliminacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Credenciales_Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Credenciales_CodigoVerificacionEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Credenciales_EmailVerificado = table.Column<bool>(type: "bit", nullable: false),
                    Credenciales_Contraseña = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Credenciales_RequiereCambioContraseña = table.Column<bool>(type: "bit", nullable: false),
                    Credenciales_SemillaMfa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Credenciales_MfaHabilitado = table.Column<bool>(type: "bit", nullable: false),
                    Credenciales_MfaVerificado = table.Column<bool>(type: "bit", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Asignaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntrenadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AlumnoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "Sesiones",
                columns: table => new
                {
                    SessionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Invalidada = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sesiones", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_Sesiones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cuestionarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContratoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFormalizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Edad = table.Column<byte>(type: "tinyint", nullable: false),
                    AlturaCm = table.Column<byte>(type: "tinyint", nullable: false),
                    MasaKilogramos = table.Column<float>(type: "real", nullable: false),
                    FrecuenciaCardiacaReposo = table.Column<byte>(type: "tinyint", nullable: false),
                    Habitos_Profesion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Habitos_Fumador = table.Column<int>(type: "int", nullable: false),
                    Habitos_Bebedor = table.Column<int>(type: "int", nullable: false),
                    Habitos_OtrasDrogas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Habitos_Deporte = table.Column<int>(type: "int", nullable: false),
                    Habitos_Sueño = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Perimetros_PectoralInspiracion = table.Column<byte>(type: "tinyint", nullable: false),
                    Perimetros_PectoralExpiracion = table.Column<byte>(type: "tinyint", nullable: false),
                    Perimetros_Abd1 = table.Column<byte>(type: "tinyint", nullable: false),
                    Perimetros_AbdominalOmbligo = table.Column<byte>(type: "tinyint", nullable: false),
                    Perimetros_Cintura = table.Column<byte>(type: "tinyint", nullable: false),
                    Perimetros_Cadera = table.Column<byte>(type: "tinyint", nullable: false),
                    Perimetros_CuadricepsMaximo = table.Column<byte>(type: "tinyint", nullable: false),
                    Perimetros_CuadricepsMinimo = table.Column<byte>(type: "tinyint", nullable: false),
                    Perimetros_Gastrocnemio = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuestionarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cuestionarios_Asignaciones_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Asignaciones",
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

            migrationBuilder.CreateIndex(
                name: "IX_Cuestionarios_ContratoId",
                table: "Cuestionarios",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_Sesiones_UsuarioId",
                table: "Sesiones",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cuestionarios");

            migrationBuilder.DropTable(
                name: "Sesiones");

            migrationBuilder.DropTable(
                name: "Asignaciones");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
