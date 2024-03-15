using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP_Management.Migrations
{
    public partial class initdatabase_v25 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_issue_issue_setting",
                table: "issue");

            migrationBuilder.DropIndex(
                name: "IX_issue_type_id",
                table: "issue");

            migrationBuilder.DropColumn(
                name: "type_id",
                table: "issue");

            migrationBuilder.AddColumn<int>(
                name: "ProcessId",
                table: "milestone",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "issue",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IssueLabel",
                columns: table => new
                {
                    IssueLabelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IssueId = table.Column<int>(type: "int", nullable: false),
                    LabelId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<short>(type: "smallint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<short>(type: "smallint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueLabel", x => x.IssueLabelId);
                    table.ForeignKey(
                        name: "FK_IssueLabel_issue_IssueId",
                        column: x => x.IssueId,
                        principalTable: "issue",
                        principalColumn: "issue_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IssueLabel_issue_setting_LabelId",
                        column: x => x.LabelId,
                        principalTable: "issue_setting",
                        principalColumn: "setting_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IssueLabel_IssueId",
                table: "IssueLabel",
                column: "IssueId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueLabel_LabelId",
                table: "IssueLabel",
                column: "LabelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IssueLabel");

            migrationBuilder.DropColumn(
                name: "ProcessId",
                table: "milestone");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "issue");

            migrationBuilder.AddColumn<int>(
                name: "type_id",
                table: "issue",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_issue_type_id",
                table: "issue",
                column: "type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_issue_issue_setting",
                table: "issue",
                column: "type_id",
                principalTable: "issue_setting",
                principalColumn: "setting_id");
        }
    }
}
