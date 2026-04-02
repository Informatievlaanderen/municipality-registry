using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace MunicipalityRegistry.Projections.Integration.Migrations
{
    /// <inheritdoc />
    public partial class AddGeometries2019 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "municipality_geometries_2019",
                schema: "integration_municipality",
                columns: table => new
                {
                    nis_code = table.Column<string>(type: "character(5)", fixedLength: true, maxLength: 5, nullable: false),
                    geometry = table.Column<Geometry>(type: "geometry", nullable: false),
                    geometry_lambert08 = table.Column<Geometry>(type: "geometry", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_municipality_geometries_2019", x => x.nis_code);
                });

            migrationBuilder.CreateIndex(
                name: "IX_municipality_geometries_2019_geometry",
                schema: "integration_municipality",
                table: "municipality_geometries_2019",
                column: "geometry")
                .Annotation("Npgsql:IndexMethod", "GIST");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_geometries_2019_geometry_lambert08",
                schema: "integration_municipality",
                table: "municipality_geometries_2019",
                column: "geometry_lambert08")
                .Annotation("Npgsql:IndexMethod", "GIST");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_geometries_2019_nis_code",
                schema: "integration_municipality",
                table: "municipality_geometries_2019",
                column: "nis_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "municipality_geometries_2019",
                schema: "integration_municipality");
        }
    }
}
