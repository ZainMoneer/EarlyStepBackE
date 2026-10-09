using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EarlyStep.DAL.Migrations
{
    /// <inheritdoc />
    public partial class TrainingPlanQuestionAnswer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityAttempts");

            migrationBuilder.DropTable(
                name: "TrainingPlanQuestions");

            migrationBuilder.AddColumn<double>(
                name: "Accuracy",
                table: "TrainingPlanActivities",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResponseTimeSeconds",
                table: "TrainingPlanActivities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TrainingPlanQuestionAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrainingPlanActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    ResponseTimeSeconds = table.Column<int>(type: "int", nullable: true),
                    AnsweredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingPlanQuestionAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingPlanQuestionAnswers_ActivityQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "ActivityQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainingPlanQuestionAnswers_TrainingPlanActivities_TrainingPlanActivityId",
                        column: x => x.TrainingPlanActivityId,
                        principalTable: "TrainingPlanActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlanQuestionAnswers_QuestionId",
                table: "TrainingPlanQuestionAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlanQuestionAnswers_TrainingPlanActivityId",
                table: "TrainingPlanQuestionAnswers",
                column: "TrainingPlanActivityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrainingPlanQuestionAnswers");

            migrationBuilder.DropColumn(
                name: "Accuracy",
                table: "TrainingPlanActivities");

            migrationBuilder.DropColumn(
                name: "ResponseTimeSeconds",
                table: "TrainingPlanActivities");

            migrationBuilder.CreateTable(
                name: "ActivityAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrainingPlanActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Accuracy = table.Column<double>(type: "float", nullable: false),
                    CorrectAnswers = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResponseTimeSeconds = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    WrongAnswers = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityAttempts_TrainingPlanActivities_TrainingPlanActivityId",
                        column: x => x.TrainingPlanActivityId,
                        principalTable: "TrainingPlanActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingPlanQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrainingPlanActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false)
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
                name: "IX_ActivityAttempts_TrainingPlanActivityId",
                table: "ActivityAttempts",
                column: "TrainingPlanActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlanQuestions_QuestionId",
                table: "TrainingPlanQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlanQuestions_TrainingPlanActivityId",
                table: "TrainingPlanQuestions",
                column: "TrainingPlanActivityId");
        }
    }
}
