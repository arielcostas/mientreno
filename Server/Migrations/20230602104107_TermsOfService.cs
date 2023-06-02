using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mientreno.Server.Migrations
{
    /// <inheritdoc />
    public partial class TermsOfService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UltimosTosAceptados",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UltimosTosAceptados",
                table: "AspNetUsers");
        }
    }
}
