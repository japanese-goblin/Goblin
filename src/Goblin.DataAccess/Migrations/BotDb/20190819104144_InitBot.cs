using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Goblin.DataAccess.Migrations
{
    public partial class InitBot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                                         "BotUsers",
                                         table => new
                                         {
                                             VkId = table.Column<long>(),
                                             WeatherCity =
                                                     table.Column<string>(maxLength: 100, nullable: true,
                                                                          defaultValue: ""),
                                             NarfuGroup = table.Column<int>(nullable: false, defaultValue: 0),
                                             IsErrorsEnabled = table.Column<bool>(nullable: false, defaultValue: true),
                                             IsAdmin = table.Column<bool>(nullable: false, defaultValue: false)
                                         },
                                         constraints: table => { table.PrimaryKey("PK_BotUsers", x => x.VkId); });

            migrationBuilder.CreateTable(
                                         "CronJobs",
                                         table => new
                                         {
                                             Id = table.Column<int>()
                                                       .Annotation("Npgsql:ValueGenerationStrategy",
                                                                   NpgsqlValueGenerationStrategy.SerialColumn),
                                             Name = table.Column<string>(),
                                             VkId = table.Column<long>(),
                                             NarfuGroup = table.Column<int>(nullable: false, defaultValue: 0),
                                             WeatherCity = table.Column<string>(nullable: true, defaultValue: ""),
                                             Hours = table.Column<int>(),
                                             Minutes = table.Column<int>()
                                         },
                                         constraints: table => { table.PrimaryKey("PK_CronJobs", x => x.Id); });

            migrationBuilder.CreateTable(
                                         "Reminds",
                                         table => new
                                         {
                                             Id = table.Column<int>()
                                                       .Annotation("Npgsql:ValueGenerationStrategy",
                                                                   NpgsqlValueGenerationStrategy.SerialColumn),
                                             BotUserId = table.Column<long>(),
                                             Text = table.Column<string>(maxLength: 100),
                                             Date = table.Column<DateTime>()
                                         },
                                         constraints: table =>
                                         {
                                             table.PrimaryKey("PK_Reminds", x => x.Id);
                                             table.ForeignKey(
                                                              "FK_Reminds_BotUsers_BotUserId",
                                                              x => x.BotUserId,
                                                              "BotUsers",
                                                              "VkId",
                                                              onDelete: ReferentialAction.Cascade);
                                         });

            migrationBuilder.CreateTable(
                                         "Subscribes",
                                         table => new
                                         {
                                             Id = table.Column<int>()
                                                       .Annotation("Npgsql:ValueGenerationStrategy",
                                                                   NpgsqlValueGenerationStrategy.SerialColumn),
                                             BotUserId = table.Column<long>(),
                                             IsWeather = table.Column<bool>(nullable: false, defaultValue: false),
                                             IsSchedule = table.Column<bool>(nullable: false, defaultValue: false)
                                         },
                                         constraints: table =>
                                         {
                                             table.PrimaryKey("PK_Subscribes", x => x.Id);
                                             table.ForeignKey(
                                                              "FK_Subscribes_BotUsers_BotUserId",
                                                              x => x.BotUserId,
                                                              "BotUsers",
                                                              "VkId",
                                                              onDelete: ReferentialAction.Cascade);
                                         });

            migrationBuilder.CreateIndex(
                                         "IX_Reminds_BotUserId",
                                         "Reminds",
                                         "BotUserId");

            migrationBuilder.CreateIndex(
                                         "IX_Subscribes_BotUserId",
                                         "Subscribes",
                                         "BotUserId",
                                         unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                                       "CronJobs");

            migrationBuilder.DropTable(
                                       "Reminds");

            migrationBuilder.DropTable(
                                       "Subscribes");

            migrationBuilder.DropTable(
                                       "BotUsers");
        }
    }
}