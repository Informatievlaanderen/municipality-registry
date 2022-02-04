using Microsoft.EntityFrameworkCore.Migrations;

namespace MunicipalityRegistry.Projections.Legacy.Migrations
{
    public partial class ColumnIndexSyndication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "CI_MunicipalitySyndication_Position",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalitySyndication",
                column: "Position")
                .Annotation("SqlServer:ColumnStoreIndex", "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "CI_MunicipalitySyndication_Position",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalitySyndication");
        }
    }
}
