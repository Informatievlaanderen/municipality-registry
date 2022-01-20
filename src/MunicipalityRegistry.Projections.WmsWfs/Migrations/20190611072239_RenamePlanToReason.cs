using Microsoft.EntityFrameworkCore.Migrations;

namespace MunicipalityRegistry.Projections.Legacy.Migrations
{
    public partial class RenamePlanToReason : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Plan",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityVersions");

            migrationBuilder.DropColumn(
                name: "Plan",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalitySyndication");

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityVersions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalitySyndication",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityVersions");

            migrationBuilder.DropColumn(
                name: "Reason",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalitySyndication");

            migrationBuilder.AddColumn<int>(
                name: "Plan",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityVersions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Plan",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalitySyndication",
                nullable: true);
        }
    }
}
