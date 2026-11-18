using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Api.Migrations
{
    /// <inheritdoc />
    public partial class CreateObservationsAndTasksTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Classes",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Classes",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Season",
                table: "Classes",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Observations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    GbifKey = table.Column<long>(type: "bigint", nullable: false),
                    Species = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    GbifClass = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Family = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Locality = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ObservedOn = table.Column<DateOnly>(type: "date", nullable: true),
                    DatasetName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    License = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IucnCategory = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Season = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Observations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Observations_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    DifficultyGroup = table.Column<int>(type: "integer", nullable: false),
                    Question = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CorrectAnswer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Options = table.Column<string[]>(type: "text[]", nullable: true),
                    ObservationId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassTasks_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassTasks_Observations_ObservationId",
                        column: x => x.ObservationId,
                        principalTable: "Observations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassTasks_ClassId",
                table: "ClassTasks",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassTasks_ObservationId",
                table: "ClassTasks",
                column: "ObservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Observations_ClassId_GbifKey",
                table: "Observations",
                columns: new[] { "ClassId", "GbifKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassTasks");

            migrationBuilder.DropTable(
                name: "Observations");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Season",
                table: "Classes");
        }
    }
}
