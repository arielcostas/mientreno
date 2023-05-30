using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class InvitacionesReutilizables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Usada",
                table: "Invitaciones");

            migrationBuilder.AddColumn<byte>(
                name: "MaximoUsos",
                table: "Invitaciones",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Usos",
                table: "Invitaciones",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaximoUsos",
                table: "Invitaciones");

            migrationBuilder.DropColumn(
                name: "Usos",
                table: "Invitaciones");

            migrationBuilder.AddColumn<bool>(
                name: "Usada",
                table: "Invitaciones",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
