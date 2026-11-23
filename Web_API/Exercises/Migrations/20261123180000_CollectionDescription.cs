using Exercises.Data.DbContext;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Exercises.Migrations
{
    /// <summary>
    /// ExerciseCollection.Category (shared by Exam through TPH) was a required free-text
    /// field that teachers took to mean the exercise type ("diagram"/"path") — which is
    /// Exercise.Category, a different column. It becomes an optional Description; existing
    /// values are kept.
    /// </summary>
    /// <remarks>
    /// Written by hand: the model snapshot dates from EF Core 3.1, so a generated migration
    /// would also carry every unrelated EF 8 model difference.
    /// </remarks>
    [DbContext(typeof(ExercisesContext))]
    [Migration("20261123180000_CollectionDescription")]
    public partial class CollectionDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Category",
                table: "ExerciseCollections",
                newName: "Description");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ExerciseCollections",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Category was NOT NULL, so rows left without a description need a value first.
            migrationBuilder.Sql(
                "UPDATE [ExerciseCollections] SET [Description] = N'' WHERE [Description] IS NULL");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ExerciseCollections",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "ExerciseCollections",
                newName: "Category");
        }
    }
}
