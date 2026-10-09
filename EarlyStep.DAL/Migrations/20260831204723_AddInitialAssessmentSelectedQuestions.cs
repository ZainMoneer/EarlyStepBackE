using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EarlyStep.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddInitialAssessmentSelectedQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "InitialAssessmentConfigs");

            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "InitialAssessmentActivities",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "InitialAssessmentQuestionAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InitialAssessmentActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    AnsweredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InitialAssessmentQuestionAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InitialAssessmentQuestionAnswers_ActivityQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "ActivityQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InitialAssessmentQuestionAnswers_InitialAssessmentActivities_InitialAssessmentActivityId",
                        column: x => x.InitialAssessmentActivityId,
                        principalTable: "InitialAssessmentActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InitialAssessmentSelectedQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InitialAssessmentConfigId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InitialAssessmentSelectedQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InitialAssessmentSelectedQuestions_ActivityQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "ActivityQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InitialAssessmentSelectedQuestions_InitialAssessmentConfigs_InitialAssessmentConfigId",
                        column: x => x.InitialAssessmentConfigId,
                        principalTable: "InitialAssessmentConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InitialAssessmentQuestionAnswers_InitialAssessmentActivityId",
                table: "InitialAssessmentQuestionAnswers",
                column: "InitialAssessmentActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_InitialAssessmentQuestionAnswers_QuestionId",
                table: "InitialAssessmentQuestionAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_InitialAssessmentSelectedQuestions_InitialAssessmentConfigId",
                table: "InitialAssessmentSelectedQuestions",
                column: "InitialAssessmentConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_InitialAssessmentSelectedQuestions_QuestionId",
                table: "InitialAssessmentSelectedQuestions",
                column: "QuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InitialAssessmentQuestionAnswers");

            migrationBuilder.DropTable(
                name: "InitialAssessmentSelectedQuestions");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "InitialAssessmentActivities");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "InitialAssessmentConfigs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
