using Microsoft.EntityFrameworkCore.Migrations;

namespace Goblin.Persistence.Migrations
{
    public partial class addiserror : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsErrorsDisabled",
                table: "Users",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsErrorsDisabled",
                table: "Users");
        }
    }
}
