using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MunicipalityRegistry.Projections.Feed.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "MunicipalityRegistryFeed");

            migrationBuilder.CreateSequence(
                name: "MunicipalityFeedSequence",
                schema: "MunicipalityRegistryFeed",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "MunicipalityDocuments",
                schema: "MunicipalityRegistryFeed",
                columns: table => new
                {
                    MunicipalityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NisCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    LastChangedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RecordCreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Document = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MunicipalityDocuments", x => x.MunicipalityId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "MunicipalityFeed",
                schema: "MunicipalityRegistryFeed",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Page = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<long>(type: "bigint", nullable: false),
                    MunicipalityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NisCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Application = table.Column<int>(type: "int", nullable: true),
                    Modification = table.Column<int>(type: "int", nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Organisation = table.Column<int>(type: "int", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CloudEvent = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MunicipalityFeed", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "ProjectionStates",
                schema: "MunicipalityRegistryFeed",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Position = table.Column<long>(type: "bigint", nullable: false),
                    DesiredState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DesiredStateChangedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectionStates", x => x.Name);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityDocuments_NisCode",
                schema: "MunicipalityRegistryFeed",
                table: "MunicipalityDocuments",
                column: "NisCode");

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityFeed_MunicipalityId",
                schema: "MunicipalityRegistryFeed",
                table: "MunicipalityFeed",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityFeed_NisCode",
                schema: "MunicipalityRegistryFeed",
                table: "MunicipalityFeed",
                column: "NisCode");

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityFeed_Page",
                schema: "MunicipalityRegistryFeed",
                table: "MunicipalityFeed",
                column: "Page");

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityFeed_Position",
                schema: "MunicipalityRegistryFeed",
                table: "MunicipalityFeed",
                column: "Position");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MunicipalityDocuments",
                schema: "MunicipalityRegistryFeed");

            migrationBuilder.DropTable(
                name: "MunicipalityFeed",
                schema: "MunicipalityRegistryFeed");

            migrationBuilder.DropTable(
                name: "ProjectionStates",
                schema: "MunicipalityRegistryFeed");

            migrationBuilder.DropSequence(
                name: "MunicipalityFeedSequence",
                schema: "MunicipalityRegistryFeed");
        }
    }
}
