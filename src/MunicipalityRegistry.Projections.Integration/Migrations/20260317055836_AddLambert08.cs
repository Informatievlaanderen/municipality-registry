using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace MunicipalityRegistry.Projections.Integration.Migrations
{
    /// <inheritdoc />
    public partial class AddLambert08 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Geometry>(
                name: "geometry_lambert08",
                schema: "integration_municipality",
                table: "municipality_geometries",
                type: "geometry",
                nullable: true);

            migrationBuilder.Sql("UPDATE integration_municipality.municipality_geometries SET geometry_lambert08 = ST_Transform(geometry, 3812);");

            migrationBuilder.Sql("ALTER TABLE integration_municipality.municipality_geometries ALTER COLUMN geometry_lambert08 SET NOT NULL;");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_geometries_geometry_lambert08",
                schema: "integration_municipality",
                table: "municipality_geometries",
                column: "geometry_lambert08")
                .Annotation("Npgsql:IndexMethod", "GIST");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_municipality_geometries_geometry_lambert08",
                schema: "integration_municipality",
                table: "municipality_geometries");

            migrationBuilder.DropColumn(
                name: "geometry_lambert08",
                schema: "integration_municipality",
                table: "municipality_geometries");
        }
    }
}
