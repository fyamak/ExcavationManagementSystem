using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Postgres.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class JobEntityRearranged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Jobs");

            migrationBuilder.AddColumn<int>(
                name: "EndHour",
                table: "Jobs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartHour",
                table: "Jobs",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndHour",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "StartHour",
                table: "Jobs");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Jobs",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);
        }
    }
}
