using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Goblin.DataAccess.Migrations.BotDb
{
    public partial class InitBotV3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                                         "BotUsers",
                                         table => new
                                         {
                                             Id = table.Column<long>(nullable: false),
                                             WeatherCity = table.Column<string>(maxLength: 100, nullable: true, defaultValue: ""),
                                             NarfuGroup = table.Column<int>(nullable: false, defaultValue: 0),
                                             IsErrorsEnabled = table.Column<bool>(nullable: false, defaultValue: true),
                                             IsAdmin = table.Column<bool>(nullable: false, defaultValue: false),
                                             HasWeatherSubscription = table.Column<bool>(nullable: false, defaultValue: false),
                                             HasScheduleSubscription = table.Column<bool>(nullable: false, defaultValue: false),
                                             UserType = table.Column<int>(nullable: false, defaultValue: 0)
                                         },
                                         constraints: table => { table.PrimaryKey("PK_BotUsers", x => x.Id); });

            migrationBuilder.CreateTable(
                                         "CronJobs",
                                         table => new
                                         {
                                             Id = table.Column<int>(nullable: false)
                                                       .Annotation("Npgsql:ValueGenerationStrategy",
                                                                   NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                                             Name = table.Column<string>(nullable: false),
                                             VkId = table.Column<long>(nullable: false),
                                             NarfuGroup = table.Column<int>(nullable: false, defaultValue: 0),
                                             WeatherCity = table.Column<string>(nullable: true, defaultValue: ""),
                                             Hours = table.Column<int>(nullable: false),
                                             Minutes = table.Column<int>(nullable: false)
                                         },
                                         constraints: table => { table.PrimaryKey("PK_CronJobs", x => x.Id); });

            migrationBuilder.CreateTable(
                                         "Reminds",
                                         table => new
                                         {
                                             Id = table.Column<int>(nullable: false)
                                                       .Annotation("Npgsql:ValueGenerationStrategy",
                                                                   NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                                             BotUserId = table.Column<long>(nullable: false),
                                             Text = table.Column<string>(maxLength: 100, nullable: false),
                                             Date = table.Column<DateTime>(nullable: false)
                                         },
                                         constraints: table =>
                                         {
                                             table.PrimaryKey("PK_Reminds", x => x.Id);
                                             table.ForeignKey(
                                                              "FK_Reminds_BotUsers_BotUserId",
                                                              x => x.BotUserId,
                                                              "BotUsers",
                                                              "Id",
                                                              onDelete: ReferentialAction.Cascade);
                                         });

            migrationBuilder.CreateIndex(
                                         "IX_Reminds_BotUserId",
                                         "Reminds",
                                         "BotUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                                       "CronJobs");

            migrationBuilder.DropTable(
                                       "Reminds");

            migrationBuilder.DropTable(
                                       "BotUsers");
        }
    }
}