using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mientreno.Server.Migrations
{
    /// <inheritdoc />
    public partial class NuevaSubç : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_PlanesSuscripcion_Suscripcion_PlanId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "PlanesSuscripcion");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Suscripcion_PlanId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Suscripcion_PlanId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "Suscripcion_CustomerId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Suscripcion_Plan",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Suscripcion_CustomerId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Suscripcion_Plan",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "Suscripcion_PlanId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PlanesSuscripcion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Modalidad = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StripePrice = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanesSuscripcion", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Suscripcion_PlanId",
                table: "AspNetUsers",
                column: "Suscripcion_PlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_PlanesSuscripcion_Suscripcion_PlanId",
                table: "AspNetUsers",
                column: "Suscripcion_PlanId",
                principalTable: "PlanesSuscripcion",
                principalColumn: "Id");
        }
    }
}
