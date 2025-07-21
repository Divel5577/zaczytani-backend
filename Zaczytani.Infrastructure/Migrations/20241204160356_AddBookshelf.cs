using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zaczytani.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBookshelf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BookShelfId",
                table: "Books",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BookShelves",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookShelves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookShelves_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_BookShelfId",
                table: "Books",
                column: "BookShelfId");

            migrationBuilder.CreateIndex(
                name: "IX_BookShelves_UserId",
                table: "BookShelves",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_BookShelves_BookShelfId",
                table: "Books",
                column: "BookShelfId",
                principalTable: "BookShelves",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_BookShelves_BookShelfId",
                table: "Books");

            migrationBuilder.DropTable(
                name: "BookShelves");

            migrationBuilder.DropIndex(
                name: "IX_Books_BookShelfId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "BookShelfId",
                table: "Books");
        }
    }
}
