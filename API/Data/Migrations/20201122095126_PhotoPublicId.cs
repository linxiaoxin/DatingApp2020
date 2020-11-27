using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class PhotoPublicId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "iSMain",
                table: "Photo",
                newName: "isMain");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Photo",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Photo");

            migrationBuilder.RenameColumn(
                name: "isMain",
                table: "Photo",
                newName: "iSMain");
        }
    }
}
