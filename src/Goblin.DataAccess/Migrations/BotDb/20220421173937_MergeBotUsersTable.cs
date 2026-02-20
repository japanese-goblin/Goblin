#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Goblin.DataAccess.Migrations.BotDb;

public partial class MergeBotUsersTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
                                     "BotUsers",
                                     table => new
                                     {
                                         Id = table.Column<long>("bigint", nullable: false),
                                         ConsumerType = table.Column<int>("integer", nullable: false),
                                         WeatherCity = table.Column<string>("character varying(100)", maxLength: 100, nullable: true,
                                                                            defaultValue: ""),
                                         NarfuGroup = table.Column<int>("integer", nullable: false, defaultValue: 0),
                                         IsErrorsEnabled = table.Column<bool>("boolean", nullable: false, defaultValue: true),
                                         IsAdmin = table.Column<bool>("boolean", nullable: false, defaultValue: false),
                                         HasWeatherSubscription = table.Column<bool>("boolean", nullable: false, defaultValue: false),
                                         HasScheduleSubscription = table.Column<bool>("boolean", nullable: false, defaultValue: false)
                                     },
                                     constraints: table =>
                                     {
                                         table.PrimaryKey("PK_BotUsers", x => new { x.Id, x.ConsumerType });
                                     });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
                                   "BotUsers");
    }
}