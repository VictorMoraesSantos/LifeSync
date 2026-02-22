using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nutrition.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addFoodTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CaloriesPerUnit",
                table: "MealFoods",
                newName: "Code");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "Potassium",
                table: "MealFoods");

            migrationBuilder.DropColumn(
                name: "Protein",
                table: "MealFoods");

            migrationBuilder.DropColumn(
                name: "Sodium",
                table: "MealFoods");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "MealFoods",
                newName: "CaloriesPerUnit");
        }
    }
}
