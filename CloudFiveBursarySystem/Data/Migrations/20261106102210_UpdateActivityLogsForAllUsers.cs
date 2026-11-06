using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudFiveBursarySystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateActivityLogsForAllUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AdminUserId",
                table: "ActivityLogs",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "AdminEmail",
                table: "ActivityLogs",
                newName: "UserEmail");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityLogs_AdminUserId",
                table: "ActivityLogs",
                newName: "IX_ActivityLogs_UserId");

            migrationBuilder.AddColumn<string>(
                name: "UserRole",
                table: "ActivityLogs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
               defaultValue: "Administrator");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_UserRole",
                table: "ActivityLogs",
                column: "UserRole");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ActivityLogs_UserRole",
                table: "ActivityLogs");

            migrationBuilder.DropColumn(
                name: "UserRole",
                table: "ActivityLogs");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ActivityLogs",
                newName: "AdminUserId");

            migrationBuilder.RenameColumn(
                name: "UserEmail",
                table: "ActivityLogs",
                newName: "AdminEmail");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityLogs_UserId",
                table: "ActivityLogs",
                newName: "IX_ActivityLogs_AdminUserId");
        }
    }
}
