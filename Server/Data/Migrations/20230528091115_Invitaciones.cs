using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class Invitaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categoria_AspNetUsers_OwnerId",
                table: "Categoria");

            migrationBuilder.DropForeignKey(
                name: "FK_Ejercicio_AspNetUsers_OwnerId",
                table: "Ejercicio");

            migrationBuilder.DropForeignKey(
                name: "FK_Ejercicio_Categoria_CategoriaId",
                table: "Ejercicio");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ejercicio",
                table: "Ejercicio");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categoria",
                table: "Categoria");

            migrationBuilder.RenameTable(
                name: "Ejercicio",
                newName: "Ejercicios");

            migrationBuilder.RenameTable(
                name: "Categoria",
                newName: "Categorias");

            migrationBuilder.RenameIndex(
                name: "IX_Ejercicio_OwnerId",
                table: "Ejercicios",
                newName: "IX_Ejercicios_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Ejercicio_CategoriaId",
                table: "Ejercicios",
                newName: "IX_Ejercicios_CategoriaId");

            migrationBuilder.RenameIndex(
                name: "IX_Categoria_OwnerId",
                table: "Categorias",
                newName: "IX_Categorias_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ejercicios",
                table: "Ejercicios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categorias",
                table: "Categorias",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Invitaciones",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntrenadorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Usada = table.Column<bool>(type: "bit", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_Invitaciones_EntrenadorId",
                table: "Invitaciones",
                column: "EntrenadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categorias_AspNetUsers_OwnerId",
                table: "Categorias",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ejercicios_AspNetUsers_OwnerId",
                table: "Ejercicios",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ejercicios_Categorias_CategoriaId",
                table: "Ejercicios",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_AspNetUsers_OwnerId",
                table: "Categorias");

            migrationBuilder.DropForeignKey(
                name: "FK_Ejercicios_AspNetUsers_OwnerId",
                table: "Ejercicios");

            migrationBuilder.DropForeignKey(
                name: "FK_Ejercicios_Categorias_CategoriaId",
                table: "Ejercicios");

            migrationBuilder.DropTable(
                name: "Invitaciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ejercicios",
                table: "Ejercicios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categorias",
                table: "Categorias");

            migrationBuilder.RenameTable(
                name: "Ejercicios",
                newName: "Ejercicio");

            migrationBuilder.RenameTable(
                name: "Categorias",
                newName: "Categoria");

            migrationBuilder.RenameIndex(
                name: "IX_Ejercicios_OwnerId",
                table: "Ejercicio",
                newName: "IX_Ejercicio_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Ejercicios_CategoriaId",
                table: "Ejercicio",
                newName: "IX_Ejercicio_CategoriaId");

            migrationBuilder.RenameIndex(
                name: "IX_Categorias_OwnerId",
                table: "Categoria",
                newName: "IX_Categoria_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ejercicio",
                table: "Ejercicio",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categoria",
                table: "Categoria",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categoria_AspNetUsers_OwnerId",
                table: "Categoria",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ejercicio_AspNetUsers_OwnerId",
                table: "Ejercicio",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ejercicio_Categoria_CategoriaId",
                table: "Ejercicio",
                column: "CategoriaId",
                principalTable: "Categoria",
                principalColumn: "Id");
        }
    }
}
