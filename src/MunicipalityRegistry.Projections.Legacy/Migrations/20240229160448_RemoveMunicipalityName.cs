using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MunicipalityRegistry.Projections.Legacy.Migrations
{
    public partial class RemoveMunicipalityName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MunicipalityName",
                schema: "MunicipalityRegistryLegacy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MunicipalityName",
                schema: "MunicipalityRegistryLegacy",
                columns: table => new
                {
                    MunicipalityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsFlemishRegion = table.Column<bool>(type: "bit", nullable: false),
                    NameDutch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameDutchSearch = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NameEnglish = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameEnglishSearch = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NameFrench = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameFrenchSearch = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NameGerman = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameGermanSearch = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NisCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    VersionTimestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MunicipalityName", x => x.MunicipalityId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityName_IsFlemishRegion",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityName",
                column: "IsFlemishRegion");

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityName_NameDutchSearch",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityName",
                column: "NameDutchSearch");

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityName_NameEnglishSearch",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityName",
                column: "NameEnglishSearch");

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityName_NameFrenchSearch",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityName",
                column: "NameFrenchSearch");

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityName_NameGermanSearch",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityName",
                column: "NameGermanSearch");

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityName_NisCode",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityName",
                column: "NisCode")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityName_VersionTimestamp",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityName",
                column: "VersionTimestamp");
        }
    }
}
