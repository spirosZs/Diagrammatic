using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Exercises.Migrations
{
    public partial class game_participations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExamParticipations",
                columns: table => new
                {
                    ExamId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamParticipations", x => new { x.ExamId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ExamParticipations_ExerciseCollections_ExamId",
                        column: x => x.ExamId,
                        principalTable: "ExerciseCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamParticipations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamParticipations_UserId",
                table: "ExamParticipations",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamParticipations");
        }
    }
}
