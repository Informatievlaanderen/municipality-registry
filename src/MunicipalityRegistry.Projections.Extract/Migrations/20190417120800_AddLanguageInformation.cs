using Microsoft.EntityFrameworkCore.Migrations;

namespace MunicipalityRegistry.Projections.Extract.Migrations
{
    public partial class AddLanguageInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameDutch",
                schema: "MunicipalityRegistryExtract",
                table: "Municipality",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEnglish",
                schema: "MunicipalityRegistryExtract",
                table: "Municipality",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameFrench",
                schema: "MunicipalityRegistryExtract",
                table: "Municipality",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameGerman",
                schema: "MunicipalityRegistryExtract",
                table: "Municipality",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficialLanguages",
                schema: "MunicipalityRegistryExtract",
                table: "Municipality",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameDutch",
                schema: "MunicipalityRegistryExtract",
                table: "Municipality");

            migrationBuilder.DropColumn(
                name: "NameEnglish",
                schema: "MunicipalityRegistryExtract",
                table: "Municipality");

            migrationBuilder.DropColumn(
                name: "NameFrench",
                schema: "MunicipalityRegistryExtract",
                table: "Municipality");

            migrationBuilder.DropColumn(
                name: "NameGerman",
                schema: "MunicipalityRegistryExtract",
                table: "Municipality");

            migrationBuilder.DropColumn(
                name: "OfficialLanguages",
                schema: "MunicipalityRegistryExtract",
                table: "Municipality");
        }
    }
}
