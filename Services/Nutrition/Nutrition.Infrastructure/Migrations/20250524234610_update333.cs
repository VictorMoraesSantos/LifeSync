using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Nutrition.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update333 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DomainEvent");

            migrationBuilder.AlterColumn<int>(
                name: "LiquidsConsumedMl",
                table: "DailyProgresses",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CaloriesConsumed",
                table: "DailyProgresses",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "LiquidsConsumedMl",
                table: "DailyProgresses",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "CaloriesConsumed",
                table: "DailyProgresses",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "DomainEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DailyProgressId = table.Column<int>(type: "integer", nullable: true),
                    DiaryId = table.Column<int>(type: "integer", nullable: true),
                    LiquidId = table.Column<int>(type: "integer", nullable: true),
                    MealFoodId = table.Column<int>(type: "integer", nullable: true),
                    MealId = table.Column<int>(type: "integer", nullable: true),
                    OccuredOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DomainEvent_DailyProgresses_DailyProgressId",
                        column: x => x.DailyProgressId,
                        principalTable: "DailyProgresses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DomainEvent_Diaries_DiaryId",
                        column: x => x.DiaryId,
                        principalTable: "Diaries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DomainEvent_Liquids_LiquidId",
                        column: x => x.LiquidId,
                        principalTable: "Liquids",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DomainEvent_MealFoods_MealFoodId",
                        column: x => x.MealFoodId,
                        principalTable: "MealFoods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DomainEvent_Meals_MealId",
                        column: x => x.MealId,
                        principalTable: "Meals",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DomainEvent_DailyProgressId",
                table: "DomainEvent",
                column: "DailyProgressId");

            migrationBuilder.CreateIndex(
                name: "IX_DomainEvent_DiaryId",
                table: "DomainEvent",
                column: "DiaryId");

            migrationBuilder.CreateIndex(
                name: "IX_DomainEvent_LiquidId",
                table: "DomainEvent",
                column: "LiquidId");

            migrationBuilder.CreateIndex(
                name: "IX_DomainEvent_MealFoodId",
                table: "DomainEvent",
                column: "MealFoodId");

            migrationBuilder.CreateIndex(
                name: "IX_DomainEvent_MealId",
                table: "DomainEvent",
                column: "MealId");
        }
    }
}
