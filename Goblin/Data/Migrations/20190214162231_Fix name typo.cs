using Microsoft.EntityFrameworkCore.Migrations;

namespace Goblin.Data.Migrations
{
    public partial class Fixnametypo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "VkID",
                table: "Reminds",
                newName: "VkId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Reminds",
                newName: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "VkId",
                table: "Reminds",
                newName: "VkID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Reminds",
                newName: "ID");
        }
    }
}
