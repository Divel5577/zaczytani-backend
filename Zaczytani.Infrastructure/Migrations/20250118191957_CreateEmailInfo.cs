using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zaczytani.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateEmailInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmailTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailTemplate = table.Column<int>(type: "int", nullable: false),
                    EmailContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentRetry = table.Column<int>(type: "int", nullable: false),
                    MaxRetries = table.Column<int>(type: "int", nullable: false),
                    IsSent = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailInfo", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailInfo");
        }
    }
}
