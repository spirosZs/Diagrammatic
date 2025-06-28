using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Exercises.Migrations
{
    public partial class game_initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateEnded",
                table: "ExerciseCollections",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateStarted",
                table: "ExerciseCollections",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParticipationCode",
                table: "ExerciseCollections",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AM",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateEnded",
                table: "ExerciseCollections");

            migrationBuilder.DropColumn(
                name: "DateStarted",
                table: "ExerciseCollections");

            migrationBuilder.DropColumn(
                name: "ParticipationCode",
                table: "ExerciseCollections");

            migrationBuilder.DropColumn(
                name: "AM",
                table: "AspNetUsers");
        }
    }
}
