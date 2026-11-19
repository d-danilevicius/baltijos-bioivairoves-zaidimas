using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherTaskFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "ClassTasks",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "Gbif");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ClassTasks",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                table: "ClassTasks");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ClassTasks");
        }
    }
}
