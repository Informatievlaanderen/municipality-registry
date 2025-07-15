using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MunicipalityRegistry.Projections.Legacy.Migrations
{
    /// <inheritdoc />
    public partial class AddIsRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRemoved",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRemoved",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityList_IsRemoved",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList",
                column: "IsRemoved");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MunicipalityList_IsRemoved",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList");

            migrationBuilder.DropColumn(
                name: "IsRemoved",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList");

            migrationBuilder.DropColumn(
                name: "IsRemoved",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityDetails");
        }
    }
}
