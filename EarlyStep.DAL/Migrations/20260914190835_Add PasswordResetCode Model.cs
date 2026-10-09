using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EarlyStep.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordResetCodeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InitialAssessmentQuestionAnswers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InitialAssessmentQuestionAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InitialAssessmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
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
                        name: "FK_InitialAssessmentQuestionAnswers_InitialAssessments_InitialAssessmentId",
                        column: x => x.InitialAssessmentId,
                        principalTable: "InitialAssessments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InitialAssessmentQuestionAnswers_InitialAssessmentId",
                table: "InitialAssessmentQuestionAnswers",
                column: "InitialAssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InitialAssessmentQuestionAnswers_QuestionId",
                table: "InitialAssessmentQuestionAnswers",
                column: "QuestionId");
        }
    }
}
