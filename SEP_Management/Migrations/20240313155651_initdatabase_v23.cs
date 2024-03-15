using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP_Management.Migrations
{
    public partial class initdatabase_v23 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "due_date",
                table: "issue",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "issue_name",
                table: "issue",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "due_date",
                table: "issue");

            migrationBuilder.DropColumn(
                name: "issue_name",
                table: "issue");
        }
    }
}
