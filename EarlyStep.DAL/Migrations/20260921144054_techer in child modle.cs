using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EarlyStep.DAL.Migrations
{
    /// <inheritdoc />
    public partial class techerinchildmodle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TeacherId",
                table: "Children",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Children_TeacherId",
                table: "Children",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Children_AspNetUsers_TeacherId",
                table: "Children",
                column: "TeacherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Children_AspNetUsers_TeacherId",
                table: "Children");

            migrationBuilder.DropIndex(
                name: "IX_Children_TeacherId",
                table: "Children");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Children");
        }
    }
}
