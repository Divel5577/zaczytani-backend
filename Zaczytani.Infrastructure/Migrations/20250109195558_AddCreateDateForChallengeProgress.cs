using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zaczytani.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreateDateForChallengeProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "ChallengeProgresses");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "ChallengeProgresses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "ChallengeProgresses");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "ChallengeProgresses",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
