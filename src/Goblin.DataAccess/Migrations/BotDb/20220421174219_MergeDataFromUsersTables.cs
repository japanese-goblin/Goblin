#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Goblin.DataAccess.Migrations.BotDb;

public partial class MergeDataFromUsersTables : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder
                .Sql(@"INSERT INTO ""BotUsers""(""Id"", ""WeatherCity"", ""NarfuGroup"", ""IsErrorsEnabled"", ""IsAdmin"", ""HasWeatherSubscription"", ""HasScheduleSubscription"", ""ConsumerType"") 
SELECT *, 0 FROM ""VkBotUsers"";");
        migrationBuilder
                .Sql(@"INSERT INTO ""BotUsers""(""Id"", ""WeatherCity"", ""NarfuGroup"", ""IsErrorsEnabled"", ""IsAdmin"", ""HasWeatherSubscription"", ""HasScheduleSubscription"", ""ConsumerType"") 
SELECT *, 1 FROM ""TgBotUsers"";");

        migrationBuilder.DropTable("TgBotUsers");
        migrationBuilder.DropTable("VkBotUsers");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
                                     "TgBotUsers",
                                     table => new
                                     {
                                         Id = table.Column<long>("bigint", nullable: false),
                                         HasScheduleSubscription = table.Column<bool>("boolean", nullable: false, defaultValue: false),
                                         HasWeatherSubscription = table.Column<bool>("boolean", nullable: false, defaultValue: false),
                                         IsAdmin = table.Column<bool>("boolean", nullable: false, defaultValue: false),
                                         IsErrorsEnabled = table.Column<bool>("boolean", nullable: false, defaultValue: true),
                                         NarfuGroup = table.Column<int>("integer", nullable: false, defaultValue: 0),
                                         WeatherCity = table.Column<string>("character varying(100)", maxLength: 100, nullable: true,
                                                                            defaultValue: "")
                                     },
                                     constraints: table =>
                                     {
                                         table.PrimaryKey("PK_TgBotUsers", x => x.Id);
                                     });

        migrationBuilder.CreateTable(
                                     "VkBotUsers",
                                     table => new
                                     {
                                         Id = table.Column<long>("bigint", nullable: false),
                                         HasScheduleSubscription = table.Column<bool>("boolean", nullable: false, defaultValue: false),
                                         HasWeatherSubscription = table.Column<bool>("boolean", nullable: false, defaultValue: false),
                                         IsAdmin = table.Column<bool>("boolean", nullable: false, defaultValue: false),
                                         IsErrorsEnabled = table.Column<bool>("boolean", nullable: false, defaultValue: true),
                                         NarfuGroup = table.Column<int>("integer", nullable: false, defaultValue: 0),
                                         WeatherCity = table.Column<string>("character varying(100)", maxLength: 100, nullable: true,
                                                                            defaultValue: "")
                                     },
                                     constraints: table =>
                                     {
                                         table.PrimaryKey("PK_VkBotUsers", x => x.Id);
                                     });
    }
}