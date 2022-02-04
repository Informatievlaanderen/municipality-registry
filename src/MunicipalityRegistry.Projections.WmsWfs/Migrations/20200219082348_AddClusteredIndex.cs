using Microsoft.EntityFrameworkCore.Migrations;

namespace MunicipalityRegistry.Projections.Legacy.Migrations
{
    public partial class AddClusteredIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MunicipalityList_DefaultName",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList");

            migrationBuilder.AlterColumn<string>(
                name: "NisCode",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityList_DefaultName",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList",
                column: "DefaultName");

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityList_NisCode",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList",
                column: "NisCode")
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MunicipalityList_DefaultName",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList");

            migrationBuilder.DropIndex(
                name: "IX_MunicipalityList_NisCode",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList");

            migrationBuilder.AlterColumn<string>(
                name: "NisCode",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityList_DefaultName",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList",
                column: "DefaultName")
                .Annotation("SqlServer:Clustered", true);
        }
    }
}
