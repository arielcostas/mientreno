using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mientreno.Server.Migrations
{
    /// <inheritdoc />
    public partial class SubGuay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_PlanesSuscripcion_Suscripcion_PlanId",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_PlanesSuscripcion_Suscripcion_PlanId",
                table: "AspNetUsers",
                column: "Suscripcion_PlanId",
                principalTable: "PlanesSuscripcion",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_PlanesSuscripcion_Suscripcion_PlanId",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_PlanesSuscripcion_Suscripcion_PlanId",
                table: "AspNetUsers",
                column: "Suscripcion_PlanId",
                principalTable: "PlanesSuscripcion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
