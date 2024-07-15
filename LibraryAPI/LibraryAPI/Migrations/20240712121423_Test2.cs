using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryAPI.Migrations
{
    public partial class Test2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "BorrowedDayLimit",
                table: "Members",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "DonorsId",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "PhysicalQuality",
                table: "Books",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "Situation",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Donors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: false),
                    Phone = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "varchar(320)", maxLength: 320, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Donors", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Donors_DonorsId",
                table: "Books");

            migrationBuilder.DropTable(
                name: "Donors");

            migrationBuilder.DropIndex(
                name: "IX_Books_DonorsId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "BorrowedDayLimit",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "DonorsId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "PhysicalQuality",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Situation",
                table: "Books");
        }
    }
}
