using Microsoft.EntityFrameworkCore.Migrations;

namespace Goblin.DataAccess.Migrations.BotDb;

public partial class AddTextToCronJob : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
                                        "CronType",
                                        "CronJobs",
                                        nullable: false,
                                        defaultValue: 2);

        migrationBuilder.AddColumn<string>(
                                           "Text",
                                           "CronJobs",
                                           maxLength: 500,
                                           nullable: true,
                                           defaultValue: "");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
                                    "CronType",
                                    "CronJobs");

        migrationBuilder.DropColumn(
                                    "Text",
                                    "CronJobs");
    }
}