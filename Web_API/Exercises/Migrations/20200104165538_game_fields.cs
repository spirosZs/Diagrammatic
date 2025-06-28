using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Exercises.Migrations
{
    public partial class game_fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CurrentExerciseId",
                table: "ExerciseCollections",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentExerciseId",
                table: "ExerciseCollections");
        }
    }
}
