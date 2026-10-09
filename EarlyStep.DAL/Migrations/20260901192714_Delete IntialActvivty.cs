using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EarlyStep.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DeleteIntialActvivty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InitialAssessmentQuestionAnswers_InitialAssessmentActivities_InitialAssessmentActivityId",
                table: "InitialAssessmentQuestionAnswers");

            migrationBuilder.DropTable(
                name: "InitialAssessmentActivities");

            migrationBuilder.RenameColumn(
                name: "InitialAssessmentActivityId",
                table: "InitialAssessmentQuestionAnswers",
                newName: "InitialAssessmentId");

            migrationBuilder.RenameIndex(
                name: "IX_InitialAssessmentQuestionAnswers_InitialAssessmentActivityId",
                table: "InitialAssessmentQuestionAnswers",
                newName: "IX_InitialAssessmentQuestionAnswers_InitialAssessmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_InitialAssessmentQuestionAnswers_InitialAssessments_InitialAssessmentId",
                table: "InitialAssessmentQuestionAnswers",
                column: "InitialAssessmentId",
                principalTable: "InitialAssessments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InitialAssessmentQuestionAnswers_InitialAssessments_InitialAssessmentId",
                table: "InitialAssessmentQuestionAnswers");

            migrationBuilder.RenameColumn(
                name: "InitialAssessmentId",
                table: "InitialAssessmentQuestionAnswers",
                newName: "InitialAssessmentActivityId");

            migrationBuilder.RenameIndex(
                name: "IX_InitialAssessmentQuestionAnswers_InitialAssessmentId",
                table: "InitialAssessmentQuestionAnswers",
                newName: "IX_InitialAssessmentQuestionAnswers_InitialAssessmentActivityId");

            migrationBuilder.CreateTable(
                name: "InitialAssessmentActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InitialAssessmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CorrectAnswers = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<double>(type: "float", nullable: false),
                    WrongAnswers = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InitialAssessmentActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InitialAssessmentActivities_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InitialAssessmentActivities_InitialAssessments_InitialAssessmentId",
                        column: x => x.InitialAssessmentId,
                        principalTable: "InitialAssessments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InitialAssessmentActivities_ActivityId",
                table: "InitialAssessmentActivities",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_InitialAssessmentActivities_InitialAssessmentId",
                table: "InitialAssessmentActivities",
                column: "InitialAssessmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_InitialAssessmentQuestionAnswers_InitialAssessmentActivities_InitialAssessmentActivityId",
                table: "InitialAssessmentQuestionAnswers",
                column: "InitialAssessmentActivityId",
                principalTable: "InitialAssessmentActivities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
