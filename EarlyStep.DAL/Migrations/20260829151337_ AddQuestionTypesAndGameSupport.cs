using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EarlyStep.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionTypesAndGameSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InitialAssessmentConfigs_Activities_ActivityId",
                table: "InitialAssessmentConfigs");

            migrationBuilder.AddColumn<int>(
                name: "CorrectOrder",
                table: "QuestionOptions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PairKey",
                table: "QuestionOptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "ActivityQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ActivityQuestions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_InitialAssessmentConfigs_Activities_ActivityId",
                table: "InitialAssessmentConfigs",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InitialAssessmentConfigs_Activities_ActivityId",
                table: "InitialAssessmentConfigs");

            migrationBuilder.DropColumn(
                name: "CorrectOrder",
                table: "QuestionOptions");

            migrationBuilder.DropColumn(
                name: "PairKey",
                table: "QuestionOptions");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "ActivityQuestions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ActivityQuestions");

            migrationBuilder.AddForeignKey(
                name: "FK_InitialAssessmentConfigs_Activities_ActivityId",
                table: "InitialAssessmentConfigs",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
