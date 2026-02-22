using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nutrition.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addFoodTable3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealFoods_Meals_MealId",
                table: "MealFoods");

            migrationBuilder.AlterColumn<int>(
                name: "MealId",
                table: "MealFoods",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_MealFoods_Meals_MealId",
                table: "MealFoods",
                column: "MealId",
                principalTable: "Meals",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MealFoods_Meals_MealId",
                table: "MealFoods");

            migrationBuilder.AlterColumn<int>(
                name: "MealId",
                table: "MealFoods",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MealFoods_Meals_MealId",
                table: "MealFoods",
                column: "MealId",
                principalTable: "Meals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
