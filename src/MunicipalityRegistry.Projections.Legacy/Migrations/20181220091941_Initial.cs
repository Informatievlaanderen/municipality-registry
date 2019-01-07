using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MunicipalityRegistry.Projections.Legacy.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "MunicipalityRegistryLegacy");

            migrationBuilder.CreateTable(
                name: "MunicipalityDetails",
                schema: "MunicipalityRegistryLegacy",
                columns: table => new
                {
                    MunicipalityId = table.Column<Guid>(nullable: false),
                    NisCode = table.Column<string>(nullable: true),
                    PrimaryLanguage = table.Column<int>(nullable: true),
                    SecondaryLanguage = table.Column<int>(nullable: true),
                    NameDutch = table.Column<string>(nullable: true),
                    NameFrench = table.Column<string>(nullable: true),
                    NameGerman = table.Column<string>(nullable: true),
                    NameEnglish = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: true),
                    VersionTimestamp = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MunicipalityDetails", x => x.MunicipalityId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "MunicipalityList",
                schema: "MunicipalityRegistryLegacy",
                columns: table => new
                {
                    MunicipalityId = table.Column<Guid>(nullable: false),
                    NisCode = table.Column<string>(nullable: true),
                    PrimaryLanguage = table.Column<int>(nullable: true),
                    SecondaryLanguage = table.Column<int>(nullable: true),
                    DefaultName = table.Column<string>(nullable: true),
                    NameDutch = table.Column<string>(nullable: true),
                    NameFrench = table.Column<string>(nullable: true),
                    NameGerman = table.Column<string>(nullable: true),
                    NameEnglish = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: true),
                    VersionTimestamp = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MunicipalityList", x => x.MunicipalityId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "MunicipalityName",
                schema: "MunicipalityRegistryLegacy",
                columns: table => new
                {
                    MunicipalityId = table.Column<Guid>(nullable: false),
                    NisCode = table.Column<string>(nullable: true),
                    NameDutch = table.Column<string>(nullable: true),
                    NameDutchSearch = table.Column<string>(nullable: true),
                    NameFrench = table.Column<string>(nullable: true),
                    NameFrenchSearch = table.Column<string>(nullable: true),
                    NameGerman = table.Column<string>(nullable: true),
                    NameGermanSearch = table.Column<string>(nullable: true),
                    NameEnglish = table.Column<string>(nullable: true),
                    NameEnglishSearch = table.Column<string>(nullable: true),
                    IsFlemishRegion = table.Column<bool>(nullable: false),
                    VersionTimestamp = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MunicipalityName", x => x.MunicipalityId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "MunicipalitySyndication",
                schema: "MunicipalityRegistryLegacy",
                columns: table => new
                {
                    Position = table.Column<long>(nullable: false),
                    MunicipalityId = table.Column<Guid>(nullable: false),
                    NisCode = table.Column<string>(nullable: true),
                    ChangeType = table.Column<string>(nullable: true),
                    PrimaryLanguage = table.Column<int>(nullable: true),
                    SecondaryLanguage = table.Column<int>(nullable: true),
                    DefaultName = table.Column<string>(nullable: true),
                    NameDutch = table.Column<string>(nullable: true),
                    NameFrench = table.Column<string>(nullable: true),
                    NameGerman = table.Column<string>(nullable: true),
                    NameEnglish = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: true),
                    RecordCreatedAt = table.Column<DateTimeOffset>(nullable: false),
                    LastChangedOn = table.Column<DateTimeOffset>(nullable: false),
                    Application = table.Column<int>(nullable: true),
                    Modification = table.Column<int>(nullable: true),
                    Operator = table.Column<string>(nullable: true),
                    Organisation = table.Column<int>(nullable: true),
                    Plan = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MunicipalitySyndication", x => x.Position)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "MunicipalityVersions",
                schema: "MunicipalityRegistryLegacy",
                columns: table => new
                {
                    MunicipalityId = table.Column<Guid>(nullable: false),
                    NisCode = table.Column<string>(nullable: true),
                    NameDutch = table.Column<string>(nullable: true),
                    NameFrench = table.Column<string>(nullable: true),
                    NameGerman = table.Column<string>(nullable: true),
                    NameEnglish = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: true),
                    Position = table.Column<long>(nullable: false),
                    VersionTimestamp = table.Column<DateTimeOffset>(nullable: true),
                    Application = table.Column<int>(nullable: true),
                    Modification = table.Column<int>(nullable: true),
                    Operator = table.Column<string>(nullable: true),
                    Organisation = table.Column<int>(nullable: true),
                    Plan = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MunicipalityVersions", x => new { x.MunicipalityId, x.Position })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ProjectionStates",
                schema: "MunicipalityRegistryLegacy",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    Position = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectionStates", x => x.Name)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityDetails_NisCode",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityDetails",
                column: "NisCode")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityList_DefaultName",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList",
                column: "DefaultName")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityList_Status",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityList",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityName_IsFlemishRegion",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityName",
                column: "IsFlemishRegion");

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

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalitySyndication_MunicipalityId",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalitySyndication",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityVersions_NisCode",
                schema: "MunicipalityRegistryLegacy",
                table: "MunicipalityVersions",
                column: "NisCode")
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MunicipalityDetails",
                schema: "MunicipalityRegistryLegacy");

            migrationBuilder.DropTable(
                name: "MunicipalityList",
                schema: "MunicipalityRegistryLegacy");

            migrationBuilder.DropTable(
                name: "MunicipalityName",
                schema: "MunicipalityRegistryLegacy");

            migrationBuilder.DropTable(
                name: "MunicipalitySyndication",
                schema: "MunicipalityRegistryLegacy");

            migrationBuilder.DropTable(
                name: "MunicipalityVersions",
                schema: "MunicipalityRegistryLegacy");

            migrationBuilder.DropTable(
                name: "ProjectionStates",
                schema: "MunicipalityRegistryLegacy");
        }
    }
}
