using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EarlyStep.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityQuestionsAndOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityOption_ActivityQuestion_QuestionId",
                table: "ActivityOption");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityQuestion_Activities_ActivityId",
                table: "ActivityQuestion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityQuestion",
                table: "ActivityQuestion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityOption",
                table: "ActivityOption");

            migrationBuilder.RenameTable(
                name: "ActivityQuestion",
                newName: "ActivityQuestions");

            migrationBuilder.RenameTable(
                name: "ActivityOption",
                newName: "ActivityOptions");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityQuestion_ActivityId",
                table: "ActivityQuestions",
                newName: "IX_ActivityQuestions_ActivityId");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityOption_QuestionId",
                table: "ActivityOptions",
                newName: "IX_ActivityOptions_QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityQuestions",
                table: "ActivityQuestions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityOptions",
                table: "ActivityOptions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityOptions_ActivityQuestions_QuestionId",
                table: "ActivityOptions",
                column: "QuestionId",
                principalTable: "ActivityQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityQuestions_Activities_ActivityId",
                table: "ActivityQuestions",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityOptions_ActivityQuestions_QuestionId",
                table: "ActivityOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityQuestions_Activities_ActivityId",
                table: "ActivityQuestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityQuestions",
                table: "ActivityQuestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityOptions",
                table: "ActivityOptions");

            migrationBuilder.RenameTable(
                name: "ActivityQuestions",
                newName: "ActivityQuestion");

            migrationBuilder.RenameTable(
                name: "ActivityOptions",
                newName: "ActivityOption");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityQuestions_ActivityId",
                table: "ActivityQuestion",
                newName: "IX_ActivityQuestion_ActivityId");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityOptions_QuestionId",
                table: "ActivityOption",
                newName: "IX_ActivityOption_QuestionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityQuestion",
                table: "ActivityQuestion",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityOption",
                table: "ActivityOption",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityOption_ActivityQuestion_QuestionId",
                table: "ActivityOption",
                column: "QuestionId",
                principalTable: "ActivityQuestion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityQuestion_Activities_ActivityId",
                table: "ActivityQuestion",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
