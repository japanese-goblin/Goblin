using Microsoft.EntityFrameworkCore.Migrations;

namespace Goblin.Data.Migrations.Main
{
    public partial class RemindTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Timestamp",
                table: "Reminds",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Reminds");
        }
    }
}