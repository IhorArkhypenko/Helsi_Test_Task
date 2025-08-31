using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Helsi.Todo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "task_list_users",
                columns: table => new
                {
                    TaskListId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_list_users", x => new { x.TaskListId, x.UserId });
                });

            migrationBuilder.CreateTable(
                name: "task_lists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_lists", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_task_list_users_TaskListId_UserId",
                table: "task_list_users",
                columns: new[] { "TaskListId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_task_lists_created_at_utc",
                table: "task_lists",
                column: "created_at_utc");

            migrationBuilder.CreateIndex(
                name: "IX_task_lists_OwnerId",
                table: "task_lists",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "task_list_users");

            migrationBuilder.DropTable(
                name: "task_lists");
        }
    }
}
