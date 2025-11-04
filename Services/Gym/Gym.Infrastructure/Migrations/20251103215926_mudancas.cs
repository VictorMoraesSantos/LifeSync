using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gym.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mudancas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompletedExercises_Exercises_ExerciseId",
                table: "CompletedExercises");

            migrationBuilder.DropIndex(
                name: "IX_CompletedExercises_ExerciseId",
                table: "CompletedExercises");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Routines");

            migrationBuilder.DropColumn(
                name: "ExerciseId",
                table: "CompletedExercises");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Routines",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExerciseId",
                table: "CompletedExercises",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CompletedExercises_ExerciseId",
                table: "CompletedExercises",
                column: "ExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompletedExercises_Exercises_ExerciseId",
                table: "CompletedExercises",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
