using Microsoft.EntityFrameworkCore.Migrations;

namespace Goblin.Migrations.Main
{
    public partial class UserRework : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityNumber",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CityNumber",
                table: "Users",
                nullable: false,
                defaultValue: 0);
        }
    }
}