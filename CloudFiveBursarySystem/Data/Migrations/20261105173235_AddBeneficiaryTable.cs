using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudFiveBursarySystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBeneficiaryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BursaryApplications_ApplicantId_BursaryProgrammeId",
                table: "BursaryApplications");

            migrationBuilder.DropIndex(
                name: "IX_BursaryApplications_ReferenceNumber",
                table: "BursaryApplications");

            migrationBuilder.CreateTable(
                name: "Beneficiaries",
                columns: table => new
                {
                    BeneficiaryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BursaryApplicationId = table.Column<int>(type: "int", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BeneficiaryStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FundingAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficiaries", x => x.BeneficiaryId);
                    table.ForeignKey(
                        name: "FK_Beneficiaries_BursaryApplications_BursaryApplicationId",
                        column: x => x.BursaryApplicationId,
                        principalTable: "BursaryApplications",
                        principalColumn: "BursaryApplicationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BursaryApplications_ApplicantId",
                table: "BursaryApplications",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_Beneficiaries_BursaryApplicationId",
                table: "Beneficiaries",
                column: "BursaryApplicationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Beneficiaries");

            migrationBuilder.DropIndex(
                name: "IX_BursaryApplications_ApplicantId",
                table: "BursaryApplications");

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
    }
}
