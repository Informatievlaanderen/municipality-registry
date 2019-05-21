using Microsoft.EntityFrameworkCore.Migrations;

namespace MunicipalityRegistry.Projections.Legacy.Migrations
{
    public partial class AddEventXmlToSyndication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventDataAsXml",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalitySyndication",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventDataAsXml",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalitySyndication");
        }
    }
}
