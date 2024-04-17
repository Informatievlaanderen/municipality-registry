using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MunicipalityRegistry.Projections.Integration.Migrations
{
    /// <inheritdoc />
    public partial class AddIsFlemishRegion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.AddColumn<bool>(
                name: "is_flemish_region",
                schema: "integration_municipality",
                table: "municipality_latest_items",
                type: "boolean",
                nullable: false,
                computedColumnSql: "nis_code LIKE '1%' OR nis_code LIKE '3%' OR nis_code LIKE '4%' OR nis_code LIKE '7%' OR nis_code LIKE '23%' OR nis_code LIKE '24%'",
                stored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_flemish_region",
                schema: "integration_municipality",
                table: "municipality_latest_items");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");
        }
    }
}
