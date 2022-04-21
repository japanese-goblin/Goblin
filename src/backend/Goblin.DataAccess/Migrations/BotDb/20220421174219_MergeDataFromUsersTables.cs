using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Goblin.DataAccess.Migrations.BotDb
{
    public partial class MergeDataFromUsersTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO ""BotUsers""(""Id"", ""WeatherCity"", ""NarfuGroup"", ""IsErrorsEnabled"", ""IsAdmin"", ""HasWeatherSubscription"", ""HasScheduleSubscription"", ""ConsumerType"") 
SELECT *, 0 FROM ""VkBotUsers"";");
            migrationBuilder.Sql(@"INSERT INTO ""BotUsers""(""Id"", ""WeatherCity"", ""NarfuGroup"", ""IsErrorsEnabled"", ""IsAdmin"", ""HasWeatherSubscription"", ""HasScheduleSubscription"", ""ConsumerType"") 
SELECT *, 1 FROM ""TgBotUsers"";");
            
            migrationBuilder.DropTable(name: "TgBotUsers");
            migrationBuilder.DropTable(name: "VkBotUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TgBotUsers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    HasScheduleSubscription = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    HasWeatherSubscription = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsErrorsEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    NarfuGroup = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    WeatherCity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TgBotUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VkBotUsers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    HasScheduleSubscription = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    HasWeatherSubscription = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsErrorsEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    NarfuGroup = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    WeatherCity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkBotUsers", x => x.Id);
                });
        }
    }
}
