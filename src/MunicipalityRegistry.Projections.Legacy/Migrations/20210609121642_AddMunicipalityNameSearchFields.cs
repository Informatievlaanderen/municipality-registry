using Microsoft.EntityFrameworkCore.Migrations;

namespace MunicipalityRegistry.Projections.Legacy.Migrations
{
    public partial class AddMunicipalityNameSearchFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameDutchSearch",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEnglishSearch",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameFrenchSearch",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameGermanSearch",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityList_NameDutchSearch",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList",
                column: "NameDutchSearch");

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityList_NameEnglishSearch",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList",
                column: "NameEnglishSearch");

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityList_NameFrenchSearch",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList",
                column: "NameFrenchSearch");

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityList_NameGermanSearch",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList",
                column: "NameGermanSearch");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MunicipalityList_NameDutchSearch",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList");

            migrationBuilder.DropIndex(
                name: "IX_MunicipalityList_NameEnglishSearch",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList");

            migrationBuilder.DropIndex(
                name: "IX_MunicipalityList_NameFrenchSearch",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList");

            migrationBuilder.DropIndex(
                name: "IX_MunicipalityList_NameGermanSearch",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList");

            migrationBuilder.DropColumn(
                name: "NameDutchSearch",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList");

            migrationBuilder.DropColumn(
                name: "NameEnglishSearch",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList");

            migrationBuilder.DropColumn(
                name: "NameFrenchSearch",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList");

            migrationBuilder.DropColumn(
                name: "NameGermanSearch",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList");
        }
    }
}
