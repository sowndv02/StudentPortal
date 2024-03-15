using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEP_Management.Migrations
{
    public partial class initdatabase_v27 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AlterColumn<string>(
                name: "semester_id",
                table: "class",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "manager_id",
                table: "class",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_class_manager_id",
                table: "class",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "IX_class_semester_id",
                table: "class",
                column: "semester_id");

            migrationBuilder.CreateIndex(
                name: "IX_class_subject_id",
                table: "class",
                column: "subject_id");

            migrationBuilder.AddForeignKey(
                name: "FK_class_Roles_semester_id",
                table: "class",
                column: "semester_id",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_class_subject_subject_id",
                table: "class",
                column: "subject_id",
                principalTable: "subject",
                principalColumn: "subject_id");

            migrationBuilder.AddForeignKey(
                name: "FK_class_Users_manager_id",
                table: "class",
                column: "manager_id",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.DropForeignKey(
                name: "FK_class_subject_subject_id",
                table: "class");

            migrationBuilder.DropIndex(
                name: "IX_class_semester_id",
                table: "class");

            migrationBuilder.DropIndex(
                name: "IX_class_subject_id",
                table: "class");
            migrationBuilder.AlterColumn<string>(
                name: "manager_id",
                table: "class",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

        }
    }
}
