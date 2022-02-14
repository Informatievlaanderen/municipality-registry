using Microsoft.EntityFrameworkCore.Migrations;

namespace MunicipalityRegistry.Projections.Wfs.Migrations
{
    public partial class Wfs_FixConvertEnumToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "wfs.municipality",
                table: "MunicipalityHelper",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                schema: "wfs.municipality",
                table: "MunicipalityHelper",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
