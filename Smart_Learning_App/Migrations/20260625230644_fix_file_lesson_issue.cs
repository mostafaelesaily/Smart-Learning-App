using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smart_Learning_App.Migrations
{
    /// <inheritdoc />
    public partial class fix_file_lesson_issue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Courses_CourseId",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "Files",
                newName: "LessonId");

            migrationBuilder.RenameIndex(
                name: "IX_Files_CourseId",
                table: "Files",
                newName: "IX_Files_LessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Lessons_LessonId",
                table: "Files",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Lessons_LessonId",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "LessonId",
                table: "Files",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Files_LessonId",
                table: "Files",
                newName: "IX_Files_CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Courses_CourseId",
                table: "Files",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
