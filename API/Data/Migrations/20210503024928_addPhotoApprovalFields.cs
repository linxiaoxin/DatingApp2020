using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class addPhotoApprovalFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "moderateDate",
                table: "Photo",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "moderatedBy",
                table: "Photo",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "moderateDate",
                table: "Photo");

            migrationBuilder.DropColumn(
                name: "moderatedBy",
                table: "Photo");
        }
    }
}
