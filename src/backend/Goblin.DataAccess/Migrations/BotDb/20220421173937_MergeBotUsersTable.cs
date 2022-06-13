using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Goblin.DataAccess.Migrations.BotDb
{
    public partial class MergeBotUsersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BotUsers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ConsumerType = table.Column<int>(type: "integer", nullable: false),
                    WeatherCity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, defaultValue: ""),
                    NarfuGroup = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsErrorsEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    HasWeatherSubscription = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    HasScheduleSubscription = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotUsers", x => new { x.Id, x.ConsumerType });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BotUsers");
        }
    }
}
