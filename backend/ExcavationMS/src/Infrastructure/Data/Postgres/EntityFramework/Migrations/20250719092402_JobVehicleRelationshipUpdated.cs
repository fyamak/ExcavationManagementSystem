using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class JobVehicleRelationshipUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobVehicle");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Customers",
                newName: "Detail");

            migrationBuilder.AddColumn<int>(
                name: "VehicleId",
                table: "Jobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_VehicleId",
                table: "Jobs",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Vehicles_VehicleId",
                table: "Jobs",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Vehicles_VehicleId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_VehicleId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "Detail",
                table: "Customers",
                newName: "Address");

            migrationBuilder.CreateTable(
                name: "JobVehicle",
                columns: table => new
                {
                    JobsId = table.Column<int>(type: "integer", nullable: false),
                    VehiclesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobVehicle", x => new { x.JobsId, x.VehiclesId });
                    table.ForeignKey(
                        name: "FK_JobVehicle_Jobs_JobsId",
                        column: x => x.JobsId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobVehicle_Vehicles_VehiclesId",
                        column: x => x.VehiclesId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobVehicle_VehiclesId",
                table: "JobVehicle",
                column: "VehiclesId");
        }
    }
}
