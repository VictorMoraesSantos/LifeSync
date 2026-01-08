using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class manylabels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskLabels_TaskItems_TaskItemId",
                table: "TaskLabels");

            migrationBuilder.DropIndex(
                name: "IX_TaskLabels_TaskItemId",
                table: "TaskLabels");

            migrationBuilder.DropColumn(
                name: "TaskItemId",
                table: "TaskLabels");

            migrationBuilder.CreateTable(
                name: "TaskItemTaskLabel",
                columns: table => new
                {
                    ItemsId = table.Column<int>(type: "integer", nullable: false),
                    LabelsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskItemTaskLabel", x => new { x.ItemsId, x.LabelsId });
                    table.ForeignKey(
                        name: "FK_TaskItemTaskLabel_TaskItems_ItemsId",
                        column: x => x.ItemsId,
                        principalTable: "TaskItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskItemTaskLabel_TaskLabels_LabelsId",
                        column: x => x.LabelsId,
                        principalTable: "TaskLabels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskItemTaskLabel_LabelsId",
                table: "TaskItemTaskLabel",
                column: "LabelsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskItemTaskLabel");

            migrationBuilder.AddColumn<int>(
                name: "TaskItemId",
                table: "TaskLabels",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskLabels_TaskItemId",
                table: "TaskLabels",
                column: "TaskItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskLabels_TaskItems_TaskItemId",
                table: "TaskLabels",
                column: "TaskItemId",
                principalTable: "TaskItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
