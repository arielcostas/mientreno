using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mientreno.Server.Migrations
{
    /// <inheritdoc />
    public partial class CorramosUnTupidoVelo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Apellidos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaEliminacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UltimosTosAceptados = table.Column<long>(type: "bigint", nullable: true),
                    EntrenadorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Suscripcion_Estado = table.Column<int>(type: "int", nullable: true),
                    Suscripcion_CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Suscripcion_Plan = table.Column<int>(type: "int", nullable: true),
                    Suscripcion_FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Suscripcion_FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Suscripcion_RenovacionAutomatica = table.Column<bool>(type: "bit", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_AspNetUsers_EntrenadorId",
                        column: x => x.EntrenadorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categorias_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cuestionarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AlumnoId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    Perimetros_AbdominalXifoides = table.Column<byte>(type: "tinyint", nullable: false),
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
                        name: "FK_Cuestionarios_AspNetUsers_AlumnoId",
                        column: x => x.AlumnoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invitaciones",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntrenadorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Usos = table.Column<byte>(type: "tinyint", nullable: false),
                    MaximoUsos = table.Column<byte>(type: "tinyint", nullable: false),
                    FechaExpiracion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invitaciones_AspNetUsers_EntrenadorId",
                        column: x => x.EntrenadorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JornadaEntrenamiento",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClienteAsignadoId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaInicioRealizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaFinRealizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaEvalucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Valoracion = table.Column<byte>(type: "tinyint", nullable: true),
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
                name: "Ejercicios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: true),
                    Dificultad = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ejercicios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ejercicios_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ejercicios_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EjercicioProgramado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JornadaId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EjercicioId = table.Column<int>(type: "int", nullable: false),
                    Series = table.Column<int>(type: "int", nullable: true),
                    Repeticiones = table.Column<int>(type: "int", nullable: true),
                    Minutos = table.Column<int>(type: "int", nullable: true)
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
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EntrenadorId",
                table: "AspNetUsers",
                column: "EntrenadorId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_OwnerId",
                table: "Categorias",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Cuestionarios_AlumnoId",
                table: "Cuestionarios",
                column: "AlumnoId");

            migrationBuilder.CreateIndex(
                name: "IX_EjercicioProgramado_EjercicioId",
                table: "EjercicioProgramado",
                column: "EjercicioId");

            migrationBuilder.CreateIndex(
                name: "IX_EjercicioProgramado_JornadaId",
                table: "EjercicioProgramado",
                column: "JornadaId");

            migrationBuilder.CreateIndex(
                name: "IX_Ejercicios_CategoriaId",
                table: "Ejercicios",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Ejercicios_OwnerId",
                table: "Ejercicios",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitaciones_EntrenadorId",
                table: "Invitaciones",
                column: "EntrenadorId");

            migrationBuilder.CreateIndex(
                name: "IX_JornadaEntrenamiento_ClienteAsignadoId",
                table: "JornadaEntrenamiento",
                column: "ClienteAsignadoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Cuestionarios");

            migrationBuilder.DropTable(
                name: "EjercicioProgramado");

            migrationBuilder.DropTable(
                name: "Invitaciones");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Ejercicios");

            migrationBuilder.DropTable(
                name: "JornadaEntrenamiento");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
