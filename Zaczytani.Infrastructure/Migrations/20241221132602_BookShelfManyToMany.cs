using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zaczytani.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BookShelfManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_BookShelves_BookShelfId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_BookShelfId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "BookShelfId",
                table: "Books");

            migrationBuilder.CreateTable(
                name: "BookBookShelf",
                columns: table => new
                {
                    BookShelfId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BooksId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookBookShelf", x => new { x.BookShelfId, x.BooksId });
                    table.ForeignKey(
                        name: "FK_BookBookShelf_BookShelves_BookShelfId",
                        column: x => x.BookShelfId,
                        principalTable: "BookShelves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookBookShelf_Books_BooksId",
                        column: x => x.BooksId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookBookShelf_BooksId",
                table: "BookBookShelf",
                column: "BooksId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookBookShelf");

            migrationBuilder.AddColumn<Guid>(
                name: "BookShelfId",
                table: "Books",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_BookShelfId",
                table: "Books",
                column: "BookShelfId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_BookShelves_BookShelfId",
                table: "Books",
                column: "BookShelfId",
                principalTable: "BookShelves",
                principalColumn: "Id");
        }
    }
}
