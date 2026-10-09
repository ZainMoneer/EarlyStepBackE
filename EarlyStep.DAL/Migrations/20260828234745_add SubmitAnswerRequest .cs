using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EarlyStep.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addSubmitAnswerRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityOptions");

            migrationBuilder.DropColumn(
                name: "ResponseTimeSeconds",
                table: "InitialAssessmentActivities");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "InitialAssessmentActivities");

            migrationBuilder.AddColumn<string>(
                name: "QuestionImageUrl",
                table: "ActivityQuestions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QuestionOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OptionText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OptionImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionOptions_ActivityQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "ActivityQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_QuestionId",
                table: "QuestionOptions",
                column: "QuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionOptions");

            migrationBuilder.DropColumn(
                name: "QuestionImageUrl",
                table: "ActivityQuestions");

            migrationBuilder.AddColumn<int>(
                name: "ResponseTimeSeconds",
                table: "InitialAssessmentActivities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "InitialAssessmentActivities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ActivityOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    OptionText = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityOptions_ActivityQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "ActivityQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityOptions_QuestionId",
                table: "ActivityOptions",
                column: "QuestionId");
        }
    }
}
