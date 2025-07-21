using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zaczytani.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAuthorsInBookRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Authors_BookRequests_BookRequestId",
                table: "Authors");

            migrationBuilder.DropIndex(
                name: "IX_Authors_BookRequestId",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "BookRequestId",
                table: "Authors");

            migrationBuilder.AddColumn<string>(
                name: "Authors",
                table: "BookRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Authors",
                table: "BookRequests");

            migrationBuilder.AddColumn<Guid>(
                name: "BookRequestId",
                table: "Authors",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Authors_BookRequestId",
                table: "Authors",
                column: "BookRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Authors_BookRequests_BookRequestId",
                table: "Authors",
                column: "BookRequestId",
                principalTable: "BookRequests",
                principalColumn: "Id");
        }
    }
}
