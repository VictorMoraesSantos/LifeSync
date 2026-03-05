using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nutrition.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_liquid_type : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Liquids_LiquidType_LiquidTypeId",
                table: "Liquids");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LiquidType",
                table: "LiquidType");

            migrationBuilder.RenameTable(
                name: "LiquidType",
                newName: "LiquidTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LiquidTypes",
                table: "LiquidTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Liquids_LiquidTypes_LiquidTypeId",
                table: "Liquids",
                column: "LiquidTypeId",
                principalTable: "LiquidTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Liquids_LiquidTypes_LiquidTypeId",
                table: "Liquids");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LiquidTypes",
                table: "LiquidTypes");

            migrationBuilder.RenameTable(
                name: "LiquidTypes",
                newName: "LiquidType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LiquidType",
                table: "LiquidType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Liquids_LiquidType_LiquidTypeId",
                table: "Liquids",
                column: "LiquidTypeId",
                principalTable: "LiquidType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
