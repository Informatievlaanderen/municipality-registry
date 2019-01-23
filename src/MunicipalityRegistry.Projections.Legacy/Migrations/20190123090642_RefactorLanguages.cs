using Microsoft.EntityFrameworkCore.Migrations;

namespace MunicipalityRegistry.Projections.Legacy.Migrations
{
    public partial class RefactorLanguages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryLanguage",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalitySyndication");

            migrationBuilder.DropColumn(
                name: "SecondaryLanguage",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalitySyndication");

            migrationBuilder.DropColumn(
                name: "PrimaryLanguage",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList");

            migrationBuilder.DropColumn(
                name: "SecondaryLanguage",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList");

            migrationBuilder.DropColumn(
                name: "PrimaryLanguage",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityDetails");

            migrationBuilder.DropColumn(
                name: "SecondaryLanguage",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityDetails");

            migrationBuilder.AddColumn<string>(
                name: "FacilitiesLanguages",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityVersions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficialLanguages",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityVersions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FacilitiesLanguages",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalitySyndication",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficialLanguages",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalitySyndication",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficialLanguages",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FacilitiesLanguages",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficialLanguages",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FacilitiesLanguages",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityVersions");

            migrationBuilder.DropColumn(
                name: "OfficialLanguages",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityVersions");

            migrationBuilder.DropColumn(
                name: "FacilitiesLanguages",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalitySyndication");

            migrationBuilder.DropColumn(
                name: "OfficialLanguages",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalitySyndication");

            migrationBuilder.DropColumn(
                name: "OfficialLanguages",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList");

            migrationBuilder.DropColumn(
                name: "FacilitiesLanguages",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityDetails");

            migrationBuilder.DropColumn(
                name: "OfficialLanguages",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityDetails");

            migrationBuilder.AddColumn<int>(
                name: "PrimaryLanguage",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalitySyndication",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SecondaryLanguage",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalitySyndication",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrimaryLanguage",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SecondaryLanguage",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrimaryLanguage",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SecondaryLanguage",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityDetails",
                nullable: true);
        }
    }
}
