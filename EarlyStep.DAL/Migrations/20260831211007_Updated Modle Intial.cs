using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EarlyStep.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedModleIntial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InitialAssessmentConfigs_Activities_ActivityId",
                table: "InitialAssessmentConfigs");

            migrationBuilder.RenameColumn(
                name: "ActivityId",
                table: "InitialAssessmentConfigs",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_InitialAssessmentConfigs_ActivityId",
                table: "InitialAssessmentConfigs",
                newName: "IX_InitialAssessmentConfigs_CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_InitialAssessmentConfigs_Categories_CategoryId",
                table: "InitialAssessmentConfigs",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InitialAssessmentConfigs_Categories_CategoryId",
                table: "InitialAssessmentConfigs");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "InitialAssessmentConfigs",
                newName: "ActivityId");

            migrationBuilder.RenameIndex(
                name: "IX_InitialAssessmentConfigs_CategoryId",
                table: "InitialAssessmentConfigs",
                newName: "IX_InitialAssessmentConfigs_ActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_InitialAssessmentConfigs_Activities_ActivityId",
                table: "InitialAssessmentConfigs",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
