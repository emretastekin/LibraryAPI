using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryAPI.Migrations
{
    public partial class Test3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookCopies_Translator_TranslatorId",
                table: "BookCopies");

            migrationBuilder.AlterColumn<int>(
                name: "TranslatorId",
                table: "BookCopies",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_BookCopies_Translator_TranslatorId",
                table: "BookCopies",
                column: "TranslatorId",
                principalTable: "Translator",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookCopies_Translator_TranslatorId",
                table: "BookCopies");

            migrationBuilder.AlterColumn<int>(
                name: "TranslatorId",
                table: "BookCopies",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BookCopies_Translator_TranslatorId",
                table: "BookCopies",
                column: "TranslatorId",
                principalTable: "Translator",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
