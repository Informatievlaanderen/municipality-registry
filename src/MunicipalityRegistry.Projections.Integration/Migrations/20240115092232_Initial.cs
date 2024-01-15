using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace MunicipalityRegistry.Projections.Integration.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "integration_municipality");

            migrationBuilder.CreateTable(
                name: "municipality_geometries",
                schema: "integration_municipality",
                columns: table => new
                {
                    nis_code = table.Column<string>(type: "character(5)", fixedLength: true, maxLength: 5, nullable: false),
                    geometry = table.Column<Geometry>(type: "geometry", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_municipality_geometries", x => x.nis_code);
                });

            migrationBuilder.CreateTable(
                name: "municipality_latest_items",
                schema: "integration_municipality",
                columns: table => new
                {
                    municipality_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nis_code = table.Column<string>(type: "character(5)", fixedLength: true, maxLength: 5, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: true),
                    oslo_status = table.Column<string>(type: "text", nullable: true),
                    official_language_dutch = table.Column<bool>(type: "boolean", nullable: true),
                    official_language_french = table.Column<bool>(type: "boolean", nullable: true),
                    official_language_german = table.Column<bool>(type: "boolean", nullable: true),
                    official_language_english = table.Column<bool>(type: "boolean", nullable: true),
                    facility_language_dutch = table.Column<bool>(type: "boolean", nullable: true),
                    facility_language_french = table.Column<bool>(type: "boolean", nullable: true),
                    facility_language_german = table.Column<bool>(type: "boolean", nullable: true),
                    facility_language_english = table.Column<bool>(type: "boolean", nullable: true),
                    name_dutch = table.Column<string>(type: "text", nullable: true),
                    name_french = table.Column<string>(type: "text", nullable: true),
                    name_german = table.Column<string>(type: "text", nullable: true),
                    name_english = table.Column<string>(type: "text", nullable: true),
                    is_removed = table.Column<bool>(type: "boolean", nullable: false),
                    puri_id = table.Column<string>(type: "text", nullable: false),
                    @namespace = table.Column<string>(name: "namespace", type: "text", nullable: false),
                    version_as_string = table.Column<string>(type: "text", nullable: false),
                    version_timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_municipality_latest_items", x => x.municipality_id);
                });

            migrationBuilder.CreateTable(
                name: "municipality_versions",
                schema: "integration_municipality",
                columns: table => new
                {
                    position = table.Column<long>(type: "bigint", nullable: false),
                    municipality_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nis_code = table.Column<string>(type: "character(5)", fixedLength: true, maxLength: 5, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: true),
                    oslo_status = table.Column<string>(type: "text", nullable: true),
                    official_language_dutch = table.Column<bool>(type: "boolean", nullable: true),
                    official_language_french = table.Column<bool>(type: "boolean", nullable: true),
                    official_language_german = table.Column<bool>(type: "boolean", nullable: true),
                    official_language_english = table.Column<bool>(type: "boolean", nullable: true),
                    facility_language_dutch = table.Column<bool>(type: "boolean", nullable: true),
                    facility_language_french = table.Column<bool>(type: "boolean", nullable: true),
                    facility_language_german = table.Column<bool>(type: "boolean", nullable: true),
                    facility_language_english = table.Column<bool>(type: "boolean", nullable: true),
                    name_dutch = table.Column<string>(type: "text", nullable: true),
                    name_french = table.Column<string>(type: "text", nullable: true),
                    name_german = table.Column<string>(type: "text", nullable: true),
                    name_english = table.Column<string>(type: "text", nullable: true),
                    is_removed = table.Column<bool>(type: "boolean", nullable: false),
                    puri_id = table.Column<string>(type: "text", nullable: false),
                    @namespace = table.Column<string>(name: "namespace", type: "text", nullable: false),
                    version_as_string = table.Column<string>(type: "text", nullable: false),
                    created_on_as_string = table.Column<string>(type: "text", nullable: false),
                    created_on_timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version_timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_municipality_versions", x => x.position);
                });

            migrationBuilder.CreateTable(
                name: "ProjectionStates",
                schema: "integration_municipality",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    Position = table.Column<long>(type: "bigint", nullable: false),
                    DesiredState = table.Column<string>(type: "text", nullable: true),
                    DesiredStateChangedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectionStates", x => x.Name);
                });

            migrationBuilder.CreateIndex(
                name: "IX_municipality_geometries_geometry",
                schema: "integration_municipality",
                table: "municipality_geometries",
                column: "geometry")
                .Annotation("Npgsql:IndexMethod", "GIST");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_geometries_nis_code",
                schema: "integration_municipality",
                table: "municipality_geometries",
                column: "nis_code");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_latest_items_is_removed",
                schema: "integration_municipality",
                table: "municipality_latest_items",
                column: "is_removed");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_latest_items_name_dutch",
                schema: "integration_municipality",
                table: "municipality_latest_items",
                column: "name_dutch");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_latest_items_name_english",
                schema: "integration_municipality",
                table: "municipality_latest_items",
                column: "name_english");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_latest_items_name_french",
                schema: "integration_municipality",
                table: "municipality_latest_items",
                column: "name_french");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_latest_items_name_german",
                schema: "integration_municipality",
                table: "municipality_latest_items",
                column: "name_german");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_latest_items_nis_code",
                schema: "integration_municipality",
                table: "municipality_latest_items",
                column: "nis_code");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_latest_items_oslo_status",
                schema: "integration_municipality",
                table: "municipality_latest_items",
                column: "oslo_status");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_latest_items_status",
                schema: "integration_municipality",
                table: "municipality_latest_items",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_versions_is_removed",
                schema: "integration_municipality",
                table: "municipality_versions",
                column: "is_removed");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_versions_municipality_id",
                schema: "integration_municipality",
                table: "municipality_versions",
                column: "municipality_id");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_versions_nis_code",
                schema: "integration_municipality",
                table: "municipality_versions",
                column: "nis_code");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_versions_version_timestamp",
                schema: "integration_municipality",
                table: "municipality_versions",
                column: "version_timestamp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "municipality_geometries",
                schema: "integration_municipality");

            migrationBuilder.DropTable(
                name: "municipality_latest_items",
                schema: "integration_municipality");

            migrationBuilder.DropTable(
                name: "municipality_versions",
                schema: "integration_municipality");

            migrationBuilder.DropTable(
                name: "ProjectionStates",
                schema: "integration_municipality");
        }
    }
}
