using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MunicipalityRegistry.Api.Import.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "MunicipalityRegistryImport");

            migrationBuilder.CreateTable(
                name: "MunicipalityMergers",
                schema: "MunicipalityRegistryImport",
                columns: table => new
                {
                    MunicipalityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    MunicipalityIdsToMergeWith = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewMunicipalityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MunicipalityMergers", x => new { x.MunicipalityId, x.Year })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MunicipalityMergers_Year",
                schema: "MunicipalityRegistryImport",
                table: "MunicipalityMergers",
                column: "Year")
                .Annotation("SqlServer:Clustered", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MunicipalityMergers",
                schema: "MunicipalityRegistryImport");
        }
    }
}
