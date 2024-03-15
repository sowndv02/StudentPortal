using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP_Management.Migrations
{
    public partial class initdatabase_v24 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "assignee_id",
                table: "issue",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "issue_type",
                table: "issue",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_issue_assignee_id",
                table: "issue",
                column: "assignee_id");

            migrationBuilder.AddForeignKey(
                name: "FK_issue_Users_assignee_id",
                table: "issue",
                column: "assignee_id",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_issue_Users_assignee_id",
                table: "issue");

            migrationBuilder.DropIndex(
                name: "IX_issue_assignee_id",
                table: "issue");

            migrationBuilder.DropColumn(
                name: "assignee_id",
                table: "issue");

            migrationBuilder.DropColumn(
                name: "issue_type",
                table: "issue");
        }
    }
}
