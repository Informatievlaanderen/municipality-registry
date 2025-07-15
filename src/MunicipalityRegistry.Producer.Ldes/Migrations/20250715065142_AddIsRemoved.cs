using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MunicipalityRegistry.Producer.Ldes.Migrations
{
    /// <inheritdoc />
    public partial class AddIsRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRemoved",
                schema: "MunicipalityRegistryProducerLdes",
                table: "Municipalities",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRemoved",
                schema: "MunicipalityRegistryProducerLdes",
                table: "Municipalities");
        }
    }
}
