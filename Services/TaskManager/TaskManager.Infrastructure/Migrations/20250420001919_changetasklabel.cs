using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changetasklabel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskLabels_TaskItems_TaskItemId",
                table: "TaskLabels");

            migrationBuilder.AlterColumn<int>(
                name: "TaskItemId",
                table: "TaskLabels",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LabelColor",
                table: "TaskLabels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "TaskLabels",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TaskLabels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskLabels_TaskItems_TaskItemId",
                table: "TaskLabels",
                column: "TaskItemId",
                principalTable: "TaskItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskLabels_TaskItems_TaskItemId",
                table: "TaskLabels");

            migrationBuilder.DropColumn(
                name: "LabelColor",
                table: "TaskLabels");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "TaskLabels");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TaskLabels");

            migrationBuilder.AlterColumn<int>(
                name: "TaskItemId",
                table: "TaskLabels",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskLabels_TaskItems_TaskItemId",
                table: "TaskLabels",
                column: "TaskItemId",
                principalTable: "TaskItems",
                principalColumn: "Id");
        }
    }
}
