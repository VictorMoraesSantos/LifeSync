using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Nutrition.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addFood : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealFoods_Meals_MealId",
                table: "MealFoods");

            migrationBuilder.DropColumn(
                name: "Calcium",
                table: "MealFoods");

            migrationBuilder.DropColumn(
                name: "Calories",
                table: "MealFoods");

            migrationBuilder.DropColumn(
                name: "Carbohydrates",
                table: "MealFoods");

            migrationBuilder.DropColumn(
                name: "Iron",
                table: "MealFoods");

            migrationBuilder.DropColumn(
                name: "Lipids",
                table: "MealFoods");

            migrationBuilder.DropColumn(
                name: "Magnesium",
                table: "MealFoods");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "MealFoods");

            migrationBuilder.DropColumn(
                name: "Potassium",
                table: "MealFoods");

            migrationBuilder.DropColumn(
                name: "Protein",
                table: "MealFoods");

            migrationBuilder.DropColumn(
                name: "Sodium",
                table: "MealFoods");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Liquids");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "MealFoods",
                newName: "FoodId");

            migrationBuilder.RenameColumn(
                name: "QuantityMl",
                table: "Liquids",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "CaloriesPerMl",
                table: "Liquids",
                newName: "LiquidTypeId");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "MealFoods",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MealId",
                table: "MealFoods",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Foods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Calories = table.Column<int>(type: "integer", nullable: false),
                    Protein = table.Column<decimal>(type: "numeric", nullable: true),
                    Lipids = table.Column<decimal>(type: "numeric", nullable: true),
                    Carbohydrates = table.Column<decimal>(type: "numeric", nullable: true),
                    Calcium = table.Column<decimal>(type: "numeric", nullable: true),
                    Magnesium = table.Column<decimal>(type: "numeric", nullable: true),
                    Iron = table.Column<decimal>(type: "numeric", nullable: true),
                    Sodium = table.Column<decimal>(type: "numeric", nullable: true),
                    Potassium = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Foods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LiquidType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiquidType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MealFoods_FoodId",
                table: "MealFoods",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_Liquids_LiquidTypeId",
                table: "Liquids",
                column: "LiquidTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Liquids_LiquidType_LiquidTypeId",
                table: "Liquids",
                column: "LiquidTypeId",
                principalTable: "LiquidType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MealFoods_Foods_FoodId",
                table: "MealFoods",
                column: "FoodId",
                principalTable: "Foods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MealFoods_Meals_MealId",
                table: "MealFoods",
                column: "MealId",
                principalTable: "Meals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Liquids_LiquidType_LiquidTypeId",
                table: "Liquids");

            migrationBuilder.DropForeignKey(
                name: "FK_MealFoods_Foods_FoodId",
                table: "MealFoods");

            migrationBuilder.DropForeignKey(
                name: "FK_MealFoods_Meals_MealId",
                table: "MealFoods");

            migrationBuilder.DropTable(
                name: "Foods");

            migrationBuilder.DropTable(
                name: "LiquidType");

            migrationBuilder.DropIndex(
                name: "IX_MealFoods_FoodId",
                table: "MealFoods");

            migrationBuilder.DropIndex(
                name: "IX_Liquids_LiquidTypeId",
                table: "Liquids");

            migrationBuilder.RenameColumn(
                name: "FoodId",
                table: "MealFoods",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Liquids",
                newName: "QuantityMl");

            migrationBuilder.RenameColumn(
                name: "LiquidTypeId",
                table: "Liquids",
                newName: "CaloriesPerMl");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "MealFoods",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "MealId",
                table: "MealFoods",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<decimal>(
                name: "Calcium",
                table: "MealFoods",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Calories",
                table: "MealFoods",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Carbohydrates",
                table: "MealFoods",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Iron",
                table: "MealFoods",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Lipids",
                table: "MealFoods",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Magnesium",
                table: "MealFoods",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "MealFoods",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Potassium",
                table: "MealFoods",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Protein",
                table: "MealFoods",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Sodium",
                table: "MealFoods",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Liquids",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_MealFoods_Meals_MealId",
                table: "MealFoods",
                column: "MealId",
                principalTable: "Meals",
                principalColumn: "Id");
        }
    }
}
