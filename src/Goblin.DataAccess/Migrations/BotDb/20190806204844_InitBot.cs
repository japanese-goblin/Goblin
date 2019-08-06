using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Goblin.DataAccess.Migrations.BotDb
{
    public partial class InitBot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BotUsers",
                columns: table => new
                {
                    VkId = table.Column<long>(nullable: false),
                    WeatherCity = table.Column<string>(maxLength: 100, nullable: true, defaultValue: ""),
                    NarfuGroup = table.Column<int>(nullable: false, defaultValue: 0),
                    IsErrorsEnabled = table.Column<bool>(nullable: false, defaultValue: true),
                    IsAdmin = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotUsers", x => x.VkId);
                });

            migrationBuilder.CreateTable(
                name: "CronJobs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    VkId = table.Column<long>(nullable: false),
                    NarfuGroup = table.Column<int>(nullable: false, defaultValue: 0),
                    WeatherCity = table.Column<string>(nullable: true, defaultValue: ""),
                    Hours = table.Column<int>(nullable: false),
                    Minutes = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CronJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reminds",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BotUserId = table.Column<long>(nullable: false),
                    Text = table.Column<string>(maxLength: 100, nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reminds_BotUsers_BotUserId",
                        column: x => x.BotUserId,
                        principalTable: "BotUsers",
                        principalColumn: "VkId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscribes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BotUserId = table.Column<long>(nullable: false),
                    IsWeather = table.Column<bool>(nullable: false, defaultValue: false),
                    IsSchedule = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscribes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscribes_BotUsers_BotUserId",
                        column: x => x.BotUserId,
                        principalTable: "BotUsers",
                        principalColumn: "VkId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reminds_BotUserId",
                table: "Reminds",
                column: "BotUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscribes_BotUserId",
                table: "Subscribes",
                column: "BotUserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CronJobs");

            migrationBuilder.DropTable(
                name: "Reminds");

            migrationBuilder.DropTable(
                name: "Subscribes");

            migrationBuilder.DropTable(
                name: "BotUsers");
        }
    }
}
