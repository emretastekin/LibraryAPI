using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryAPI.Migrations
{
    public partial class Test5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookCopies_Books_BookId",
                table: "BookCopies");

            migrationBuilder.DropForeignKey(
                name: "FK_BookCopies_Publishers_PublisherId",
                table: "BookCopies");

            migrationBuilder.RenameColumn(
                name: "IsAvaliable",
                table: "BookCopies",
                newName: "IsAvailable");

            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "BookCopies",
                newName: "PublisherId1");

            migrationBuilder.RenameIndex(
                name: "IX_BookCopies_BookId",
                table: "BookCopies",
                newName: "IX_BookCopies_PublisherId1");

            migrationBuilder.AlterColumn<int>(
                name: "PublisherId",
                table: "BookCopies",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LocationShelf",
                table: "BookCopies",
                type: "varchar(6)",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "BooksId",
                table: "BookCopies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LocationShelf1",
                table: "BookCopies",
                type: "varchar(6)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookCopies_BooksId",
                table: "BookCopies",
                column: "BooksId");

            migrationBuilder.CreateIndex(
                name: "IX_BookCopies_LocationShelf",
                table: "BookCopies",
                column: "LocationShelf");

            migrationBuilder.CreateIndex(
                name: "IX_BookCopies_LocationShelf1",
                table: "BookCopies",
                column: "LocationShelf1");

            migrationBuilder.AddForeignKey(
                name: "FK_BookCopies_Books_BooksId",
                table: "BookCopies",
                column: "BooksId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookCopies_Locations_LocationShelf",
                table: "BookCopies",
                column: "LocationShelf",
                principalTable: "Locations",
                principalColumn: "Shelf",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookCopies_Locations_LocationShelf1",
                table: "BookCopies",
                column: "LocationShelf1",
                principalTable: "Locations",
                principalColumn: "Shelf");

            migrationBuilder.AddForeignKey(
                name: "FK_BookCopies_Publishers_PublisherId",
                table: "BookCopies",
                column: "PublisherId",
                principalTable: "Publishers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookCopies_Publishers_PublisherId1",
                table: "BookCopies",
                column: "PublisherId1",
                principalTable: "Publishers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookCopies_Books_BooksId",
                table: "BookCopies");

            migrationBuilder.DropForeignKey(
                name: "FK_BookCopies_Locations_LocationShelf",
                table: "BookCopies");

            migrationBuilder.DropForeignKey(
                name: "FK_BookCopies_Locations_LocationShelf1",
                table: "BookCopies");

            migrationBuilder.DropForeignKey(
                name: "FK_BookCopies_Publishers_PublisherId",
                table: "BookCopies");

            migrationBuilder.DropForeignKey(
                name: "FK_BookCopies_Publishers_PublisherId1",
                table: "BookCopies");

            migrationBuilder.DropIndex(
                name: "IX_BookCopies_BooksId",
                table: "BookCopies");

            migrationBuilder.DropIndex(
                name: "IX_BookCopies_LocationShelf",
                table: "BookCopies");

            migrationBuilder.DropIndex(
                name: "IX_BookCopies_LocationShelf1",
                table: "BookCopies");

            migrationBuilder.DropColumn(
                name: "BooksId",
                table: "BookCopies");

            migrationBuilder.DropColumn(
                name: "LocationShelf1",
                table: "BookCopies");

            migrationBuilder.RenameColumn(
                name: "PublisherId1",
                table: "BookCopies",
                newName: "BookId");

            migrationBuilder.RenameColumn(
                name: "IsAvailable",
                table: "BookCopies",
                newName: "IsAvaliable");

            migrationBuilder.RenameIndex(
                name: "IX_BookCopies_PublisherId1",
                table: "BookCopies",
                newName: "IX_BookCopies_BookId");

            migrationBuilder.AlterColumn<int>(
                name: "PublisherId",
                table: "BookCopies",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "LocationShelf",
                table: "BookCopies",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(6)",
                oldMaxLength: 6);

            migrationBuilder.AddForeignKey(
                name: "FK_BookCopies_Books_BookId",
                table: "BookCopies",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookCopies_Publishers_PublisherId",
                table: "BookCopies",
                column: "PublisherId",
                principalTable: "Publishers",
                principalColumn: "Id");
        }
    }
}
