using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EarlyStep.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingPlanQuestionAnswers_TrainingPlanActivities_TrainingPlanActivityId",
                table: "TrainingPlanQuestionAnswers");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingPlanQuestionAnswers_TrainingPlanActivities_TrainingPlanActivityId",
                table: "TrainingPlanQuestionAnswers",
                column: "TrainingPlanActivityId",
                principalTable: "TrainingPlanActivities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingPlanQuestionAnswers_TrainingPlanActivities_TrainingPlanActivityId",
                table: "TrainingPlanQuestionAnswers");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingPlanQuestionAnswers_TrainingPlanActivities_TrainingPlanActivityId",
                table: "TrainingPlanQuestionAnswers",
                column: "TrainingPlanActivityId",
                principalTable: "TrainingPlanActivities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
