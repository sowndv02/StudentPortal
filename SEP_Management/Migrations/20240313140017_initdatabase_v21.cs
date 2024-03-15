using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP_Management.Migrations
{
    public partial class initdatabase_v21 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "gitlab_id",
                table: "issue_setting",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "gitlab_id",
                table: "issue",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "gitlab_id",
                table: "issue_setting");

            migrationBuilder.DropColumn(
                name: "gitlab_id",
                table: "issue");
        }
    }
}
