using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyWebApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Exercise",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    MaxReps = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxWeight = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercise", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workout",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workout", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workout_exercises",
                columns: table => new
                {
                    workout_id = table.Column<int>(type: "INTEGER", nullable: false),
                    exercise_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workout_exercises", x => new { x.workout_id, x.exercise_id });
                    table.ForeignKey(
                        name: "FK_Workout_exercises_Exercise_exercise_id",
                        column: x => x.exercise_id,
                        principalTable: "Exercise",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Workout_exercises_Workout_workout_id",
                        column: x => x.workout_id,
                        principalTable: "Workout",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Workout_progress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    workout_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workout_progress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workout_progress_Workout_workout_id",
                        column: x => x.workout_id,
                        principalTable: "Workout",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exercise_progress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    workout_prog_id = table.Column<int>(type: "INTEGER", nullable: false),
                    exercise_id = table.Column<int>(type: "INTEGER", nullable: false),
                    Set = table.Column<int>(type: "INTEGER", nullable: false),
                    Reps = table.Column<int>(type: "INTEGER", nullable: false),
                    Weight = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercise_progress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exercise_progress_Exercise_exercise_id",
                        column: x => x.exercise_id,
                        principalTable: "Exercise",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Exercise_progress_Workout_progress_workout_prog_id",
                        column: x => x.workout_prog_id,
                        principalTable: "Workout_progress",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Exercise",
                columns: new[] { "Id", "MaxReps", "MaxWeight", "Name" },
                values: new object[,]
                {
                    { 1, 50, null, "Push-ups" },
                    { 2, null, 100.0, "Bench Press" },
                    { 3, null, 150.0, "Squats" }
                });

            migrationBuilder.InsertData(
                table: "Workout",
                columns: new[] { "Id", "Title" },
                values: new object[,]
                {
                    { 1, "Upper Body" },
                    { 2, "Lower Body" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exercise_progress_exercise_id",
                table: "Exercise_progress",
                column: "exercise_id");

            migrationBuilder.CreateIndex(
                name: "IX_Exercise_progress_workout_prog_id",
                table: "Exercise_progress",
                column: "workout_prog_id");

            migrationBuilder.CreateIndex(
                name: "IX_Workout_exercises_exercise_id",
                table: "Workout_exercises",
                column: "exercise_id");

            migrationBuilder.CreateIndex(
                name: "IX_Workout_progress_workout_id",
                table: "Workout_progress",
                column: "workout_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Exercise_progress");

            migrationBuilder.DropTable(
                name: "Workout_exercises");

            migrationBuilder.DropTable(
                name: "Workout_progress");

            migrationBuilder.DropTable(
                name: "Exercise");

            migrationBuilder.DropTable(
                name: "Workout");
        }
    }
}
