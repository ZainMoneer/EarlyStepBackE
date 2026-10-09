using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EarlyStep.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInitialAssessmentConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnsweredAt",
                table: "InitialAssessmentQuestionAnswers");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "InitialAssessmentSelectedQuestions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "InitialAssessmentConfigs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "InitialAssessmentSelectedQuestions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "InitialAssessmentConfigs");

            migrationBuilder.AddColumn<DateTime>(
                name: "AnsweredAt",
                table: "InitialAssessmentQuestionAnswers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
