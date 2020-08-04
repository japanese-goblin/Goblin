using Microsoft.EntityFrameworkCore.Migrations;

namespace Goblin.DataAccess.Migrations.BotDb
{
    public partial class AddTextToCronJob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CronType",
                table: "CronJobs",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "CronJobs",
                maxLength: 500,
                nullable: true,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CronType",
                table: "CronJobs");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "CronJobs");
        }
    }
}
