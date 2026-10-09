using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EarlyStep.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MakeActivitySlugUniqueForActiveActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Activities_Slug",
                table: "Activities");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_Slug",
                table: "Activities",
                column: "Slug",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Activities_Slug",
                table: "Activities");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_Slug",
                table: "Activities",
                column: "Slug",
                unique: true);
        }
    }
}
