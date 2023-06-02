using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mientreno.Server.Migrations
{
    /// <inheritdoc />
    public partial class PlanesSuscripcion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.DropColumn(
		        "Suscripcion_Plan", 
		        "AspNetUsers");
	        
            migrationBuilder.AddColumn<int>(
                name: "Suscripcion_Plan",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.DropColumn(
		        "Suscripcion_Plan",
		        "AspNetUsers");

	        migrationBuilder.AddColumn<string>(
		        name: "Suscripcion_Plan",
		        table: "AspNetUsers",
		        type: "nvarchar(max)",
		        nullable: true);
        }
    }
}
