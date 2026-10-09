using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EarlyStep.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveResponseTimeSecondsandAccuracy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accuracy",
                table: "TrainingPlanActivities");

            migrationBuilder.DropColumn(
                name: "ResponseTimeSeconds",
                table: "TrainingPlanActivities");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Accuracy",
                table: "TrainingPlanActivities",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResponseTimeSeconds",
                table: "TrainingPlanActivities",
                type: "int",
                nullable: true);
        }
    }
}
