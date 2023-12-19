﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MunicipalityRegistry.Projections.Integration.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "integration");

            migrationBuilder.CreateTable(
                name: "municipality_latest_items",
                schema: "integration",
                columns: table => new
                {
                    municipality_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nis_code = table.Column<string>(type: "character(5)", fixedLength: true, maxLength: 5, nullable: false),
                    status = table.Column<string>(type: "text", nullable: true),
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
                    version_timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    idempotence_key = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_municipality_latest_items", x => x.municipality_id);
                });

            migrationBuilder.CreateTable(
                name: "municipality_versions",
                schema: "integration",
                columns: table => new
                {
                    position = table.Column<long>(type: "bigint", nullable: false),
                    municipality_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nis_code = table.Column<string>(type: "character(5)", fixedLength: true, maxLength: 5, nullable: false),
                    status = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_municipality_versions", x => x.position);
                });

            migrationBuilder.CreateTable(
                name: "ProjectionStates",
                schema: "integration",
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
                name: "IX_municipality_latest_items_is_removed",
                schema: "integration",
                table: "municipality_latest_items",
                column: "is_removed");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_latest_items_name_dutch",
                schema: "integration",
                table: "municipality_latest_items",
                column: "name_dutch");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_latest_items_name_english",
                schema: "integration",
                table: "municipality_latest_items",
                column: "name_english");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_latest_items_name_french",
                schema: "integration",
                table: "municipality_latest_items",
                column: "name_french");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_latest_items_name_german",
                schema: "integration",
                table: "municipality_latest_items",
                column: "name_german");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_latest_items_nis_code",
                schema: "integration",
                table: "municipality_latest_items",
                column: "nis_code");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_latest_items_status",
                schema: "integration",
                table: "municipality_latest_items",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_versions_is_removed",
                schema: "integration",
                table: "municipality_versions",
                column: "is_removed");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_versions_municipality_id",
                schema: "integration",
                table: "municipality_versions",
                column: "municipality_id");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_versions_name_dutch",
                schema: "integration",
                table: "municipality_versions",
                column: "name_dutch");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_versions_name_english",
                schema: "integration",
                table: "municipality_versions",
                column: "name_english");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_versions_name_french",
                schema: "integration",
                table: "municipality_versions",
                column: "name_french");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_versions_name_german",
                schema: "integration",
                table: "municipality_versions",
                column: "name_german");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_versions_nis_code",
                schema: "integration",
                table: "municipality_versions",
                column: "nis_code");

            migrationBuilder.CreateIndex(
                name: "IX_municipality_versions_status",
                schema: "integration",
                table: "municipality_versions",
                column: "status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "municipality_latest_items",
                schema: "integration");

            migrationBuilder.DropTable(
                name: "municipality_versions",
                schema: "integration");

            migrationBuilder.DropTable(
                name: "ProjectionStates",
                schema: "integration");
        }
    }
}
