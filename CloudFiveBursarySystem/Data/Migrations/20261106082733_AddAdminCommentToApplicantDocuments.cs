using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudFiveBursarySystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminCommentToApplicantDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminComment",
                table: "ApplicantDocuments",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminComment",
                table: "ApplicantDocuments");
        }
    }
}
