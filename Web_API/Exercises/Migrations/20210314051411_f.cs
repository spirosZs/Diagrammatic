using Microsoft.EntityFrameworkCore.Migrations;

namespace Exercises.Migrations
{
    public partial class f : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ExerciseCollections_CurrentExerciseId",
                table: "ExerciseCollections",
                column: "CurrentExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseCollections_Exercises_CurrentExerciseId",
                table: "ExerciseCollections",
                column: "CurrentExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseCollections_Exercises_CurrentExerciseId",
                table: "ExerciseCollections");

            migrationBuilder.DropIndex(
                name: "IX_ExerciseCollections_CurrentExerciseId",
                table: "ExerciseCollections");
        }
    }
}
