using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryAPI.Migrations
{
    public partial class Test6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Donors_DonorsId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_DonorsId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "DonorsId",
                table: "Books");

            migrationBuilder.AddColumn<string>(
                name: "Place",
                table: "Publishers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DonationDate",
                table: "Donors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Series",
                table: "Books",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TranslatorId",
                table: "BookCopies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BookDonor",
                columns: table => new
                {
                    BooksId = table.Column<int>(type: "int", nullable: false),
                    DonorsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookDonor", x => new { x.BooksId, x.DonorsId });
                    table.ForeignKey(
                        name: "FK_BookDonor_Books_BooksId",
                        column: x => x.BooksId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookDonor_Donors_DonorsId",
                        column: x => x.DonorsId,
                        principalTable: "Donors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Translator",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(746)", maxLength: 746, nullable: false),
                    Biography = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translator", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookTranslator",
                columns: table => new
                {
                    BooksId = table.Column<int>(type: "int", nullable: false),
                    TranslatorsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookTranslator", x => new { x.BooksId, x.TranslatorsId });
                    table.ForeignKey(
                        name: "FK_BookTranslator_Books_BooksId",
                        column: x => x.BooksId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookTranslator_Translator_TranslatorsId",
                        column: x => x.TranslatorsId,
                        principalTable: "Translator",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookCopies_TranslatorId",
                table: "BookCopies",
                column: "TranslatorId");

            migrationBuilder.CreateIndex(
                name: "IX_BookDonor_DonorsId",
                table: "BookDonor",
                column: "DonorsId");

            migrationBuilder.CreateIndex(
                name: "IX_BookTranslator_TranslatorsId",
                table: "BookTranslator",
                column: "TranslatorsId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookCopies_Translator_TranslatorId",
                table: "BookCopies",
                column: "TranslatorId",
                principalTable: "Translator",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookCopies_Translator_TranslatorId",
                table: "BookCopies");

            migrationBuilder.DropTable(
                name: "BookDonor");

            migrationBuilder.DropTable(
                name: "BookTranslator");

            migrationBuilder.DropTable(
                name: "Translator");

            migrationBuilder.DropIndex(
                name: "IX_BookCopies_TranslatorId",
                table: "BookCopies");

            migrationBuilder.DropColumn(
                name: "Place",
                table: "Publishers");

            migrationBuilder.DropColumn(
                name: "DonationDate",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "Series",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "TranslatorId",
                table: "BookCopies");

            migrationBuilder.AddColumn<int>(
                name: "DonorsId",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_DonorsId",
                table: "Books",
                column: "DonorsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Donors_DonorsId",
                table: "Books",
                column: "DonorsId",
                principalTable: "Donors",
                principalColumn: "Id");
        }
    }
}
