using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudFiveBursarySystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateBursaryProgrammeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BursaryProgrammes",
                columns: table => new
                {
                    BursaryProgrammeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProgrammeName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    EligibilityCriteria = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FundingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ApplicationOpeningDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApplicationClosingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProgrammeStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BursaryProgrammes", x => x.BursaryProgrammeId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BursaryProgrammes");
        }
    }
}
