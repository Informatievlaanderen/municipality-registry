using System;
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
                name: "municipalities2",
                schema: "integration",
                columns: table => new
                {
                    MunicipalityId = table.Column<Guid>(type: "uuid", nullable: false),
                    NisCode = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: true),
                    OfficialLanguageDutch = table.Column<bool>(type: "boolean", nullable: true),
                    OfficialLanguageFrench = table.Column<bool>(type: "boolean", nullable: true),
                    OfficialLanguageGerman = table.Column<bool>(type: "boolean", nullable: true),
                    OfficialLanguageEnglish = table.Column<bool>(type: "boolean", nullable: true),
                    FacilityLanguageDutch = table.Column<bool>(type: "boolean", nullable: true),
                    FacilityLanguageFrench = table.Column<bool>(type: "boolean", nullable: true),
                    FacilityLanguageGerman = table.Column<bool>(type: "boolean", nullable: true),
                    FacilityLanguageEnglish = table.Column<bool>(type: "boolean", nullable: true),
                    NameDutch = table.Column<string>(type: "text", nullable: true),
                    NameFrench = table.Column<string>(type: "text", nullable: true),
                    NameGerman = table.Column<string>(type: "text", nullable: true),
                    NameEnglish = table.Column<string>(type: "text", nullable: true),
                    IsRemoved = table.Column<bool>(type: "boolean", nullable: false),
                    PuriId = table.Column<string>(type: "text", nullable: true),
                    Namespace = table.Column<string>(type: "text", nullable: true),
                    VersionString = table.Column<string>(type: "text", nullable: true),
                    VersionTimestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IdempotenceKey = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_municipalities2", x => x.MunicipalityId);
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
                name: "IX_municipalities2_IsRemoved",
                schema: "integration",
                table: "municipalities2",
                column: "IsRemoved");

            migrationBuilder.CreateIndex(
                name: "IX_municipalities2_NameDutch",
                schema: "integration",
                table: "municipalities2",
                column: "NameDutch");

            migrationBuilder.CreateIndex(
                name: "IX_municipalities2_NameEnglish",
                schema: "integration",
                table: "municipalities2",
                column: "NameEnglish");

            migrationBuilder.CreateIndex(
                name: "IX_municipalities2_NameFrench",
                schema: "integration",
                table: "municipalities2",
                column: "NameFrench");

            migrationBuilder.CreateIndex(
                name: "IX_municipalities2_NameGerman",
                schema: "integration",
                table: "municipalities2",
                column: "NameGerman");

            migrationBuilder.CreateIndex(
                name: "IX_municipalities2_NisCode",
                schema: "integration",
                table: "municipalities2",
                column: "NisCode");

            migrationBuilder.CreateIndex(
                name: "IX_municipalities2_Status",
                schema: "integration",
                table: "municipalities2",
                column: "Status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "municipalities2",
                schema: "integration");

            migrationBuilder.DropTable(
                name: "ProjectionStates",
                schema: "integration");
        }
    }
}
