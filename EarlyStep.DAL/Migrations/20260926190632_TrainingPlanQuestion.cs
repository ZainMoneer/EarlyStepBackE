using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EarlyStep.DAL.Migrations
{
    /// <inheritdoc />
    public partial class TrainingPlanQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "TrainingPlanActivities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CorrectAnswers",
                table: "TrainingPlanActivities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "TrainingPlanActivities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "TrainingPlanActivities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WrongAnswers",
                table: "TrainingPlanActivities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TrainingPlanQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrainingPlanActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingPlanQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingPlanQuestions_ActivityQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "ActivityQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingPlanQuestions_TrainingPlanActivities_TrainingPlanActivityId",
                        column: x => x.TrainingPlanActivityId,
                        principalTable: "TrainingPlanActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlanQuestions_QuestionId",
                table: "TrainingPlanQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlanQuestions_TrainingPlanActivityId",
                table: "TrainingPlanQuestions",
                column: "TrainingPlanActivityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrainingPlanQuestions");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "TrainingPlanActivities");

            migrationBuilder.DropColumn(
                name: "CorrectAnswers",
                table: "TrainingPlanActivities");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "TrainingPlanActivities");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "TrainingPlanActivities");

            migrationBuilder.DropColumn(
                name: "WrongAnswers",
                table: "TrainingPlanActivities");
        }
    }
}
