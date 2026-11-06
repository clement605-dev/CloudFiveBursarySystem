using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudFiveBursarySystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationReferenceNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BursaryApplications_ApplicantId",
                table: "BursaryApplications");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNumber",
                table: "BursaryApplications",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BursaryApplications_ApplicantId_BursaryProgrammeId",
                table: "BursaryApplications",
                columns: new[] { "ApplicantId", "BursaryProgrammeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BursaryApplications_ReferenceNumber",
                table: "BursaryApplications",
                column: "ReferenceNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BursaryApplications_ApplicantId_BursaryProgrammeId",
                table: "BursaryApplications");

            migrationBuilder.DropIndex(
                name: "IX_BursaryApplications_ReferenceNumber",
                table: "BursaryApplications");

            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                table: "BursaryApplications");

            migrationBuilder.CreateIndex(
                name: "IX_BursaryApplications_ApplicantId",
                table: "BursaryApplications",
                column: "ApplicantId");
        }
    }
}
