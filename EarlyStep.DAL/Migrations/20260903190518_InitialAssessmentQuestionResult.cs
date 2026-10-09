using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EarlyStep.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialAssessmentQuestionResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InitialAssessmentQuestionResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InitialAssessmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SelectedOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InitialAssessmentQuestionResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InitialAssessmentQuestionResults_ActivityQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "ActivityQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InitialAssessmentQuestionResults_InitialAssessments_InitialAssessmentId",
                        column: x => x.InitialAssessmentId,
                        principalTable: "InitialAssessments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InitialAssessmentQuestionResults_InitialAssessmentId",
                table: "InitialAssessmentQuestionResults",
                column: "InitialAssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InitialAssessmentQuestionResults_QuestionId",
                table: "InitialAssessmentQuestionResults",
                column: "QuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InitialAssessmentQuestionResults");
        }
    }
}
