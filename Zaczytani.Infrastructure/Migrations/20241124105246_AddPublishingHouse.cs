using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zaczytani.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPublishingHouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookRequests_AspNetUsers_CreatedById",
                table: "BookRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_AspNetUsers_CreatedById",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Books",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Books_CreatedById",
                table: "Books",
                newName: "IX_Books_UserId");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "BookRequests",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BookRequests_CreatedById",
                table: "BookRequests",
                newName: "IX_BookRequests_UserId");

            migrationBuilder.AddColumn<string>(
                name: "Genre",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<Guid>(
                name: "PublishingHouseId",
                table: "Books",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateOnly>(
                name: "ReleaseDate",
                table: "Books",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "Series",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Genre",
                table: "BookRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "PublishingHouse",
                table: "BookRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Series",
                table: "BookRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PublishingHouses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishingHouses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_PublishingHouseId",
                table: "Books",
                column: "PublishingHouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookRequests_AspNetUsers_UserId",
                table: "BookRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AspNetUsers_UserId",
                table: "Books",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_PublishingHouses_PublishingHouseId",
                table: "Books",
                column: "PublishingHouseId",
                principalTable: "PublishingHouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookRequests_AspNetUsers_UserId",
                table: "BookRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_AspNetUsers_UserId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_PublishingHouses_PublishingHouseId",
                table: "Books");

            migrationBuilder.DropTable(
                name: "PublishingHouses");

            migrationBuilder.DropIndex(
                name: "IX_Books_PublishingHouseId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Genre",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "PublishingHouseId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Series",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Genre",
                table: "BookRequests");

            migrationBuilder.DropColumn(
                name: "PublishingHouse",
                table: "BookRequests");

            migrationBuilder.DropColumn(
                name: "Series",
                table: "BookRequests");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Books",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Books_UserId",
                table: "Books",
                newName: "IX_Books_CreatedById");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BookRequests",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_BookRequests_UserId",
                table: "BookRequests",
                newName: "IX_BookRequests_CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_BookRequests_AspNetUsers_CreatedById",
                table: "BookRequests",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AspNetUsers_CreatedById",
                table: "Books",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
