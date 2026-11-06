using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudFiveBursarySystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateBursaryApplicationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BursaryApplications",
                columns: table => new
                {
                    BursaryApplicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicantId = table.Column<int>(type: "int", nullable: false),
                    BursaryProgrammeId = table.Column<int>(type: "int", nullable: false),
                    DateSubmitted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApplicationStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AdministratorComments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BursaryApplications", x => x.BursaryApplicationId);
                    table.ForeignKey(
                        name: "FK_BursaryApplications_Applicants_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "Applicants",
                        principalColumn: "ApplicantId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BursaryApplications_BursaryProgrammes_BursaryProgrammeId",
                        column: x => x.BursaryProgrammeId,
                        principalTable: "BursaryProgrammes",
                        principalColumn: "BursaryProgrammeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BursaryApplications_ApplicantId",
                table: "BursaryApplications",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_BursaryApplications_BursaryProgrammeId",
                table: "BursaryApplications",
                column: "BursaryProgrammeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BursaryApplications");
        }
    }
}
