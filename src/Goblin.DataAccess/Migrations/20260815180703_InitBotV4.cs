using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Goblin.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitBotV4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BotUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WeatherCity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NarfuGroup = table.Column<int>(type: "integer", nullable: true),
                    IsErrorsEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    HasWeatherSubscription = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    HasScheduleSubscription = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ConsumerId = table.Column<long>(type: "bigint", nullable: false),
                    ConsumerType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CronJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    NarfuGroup = table.Column<int>(type: "integer", nullable: true),
                    WeatherCity = table.Column<string>(type: "text", nullable: true),
                    Text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Time_Minute = table.Column<string>(type: "text", nullable: false),
                    Time_Hour = table.Column<string>(type: "text", nullable: false),
                    Time_DayOfMonth = table.Column<string>(type: "text", nullable: false),
                    Time_Month = table.Column<string>(type: "text", nullable: false),
                    Time_DayOfWeek = table.Column<string>(type: "text", nullable: false),
                    CronType = table.Column<int>(type: "integer", nullable: false),
                    ConsumerType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CronJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotUserSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BotUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FlowType = table.Column<int>(type: "integer", nullable: false),
                    FlowStepType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotUserSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BotUserSessions_BotUsers_BotUserId",
                        column: x => x.BotUserId,
                        principalTable: "BotUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reminds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    BotUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reminds_BotUsers_BotUserId",
                        column: x => x.BotUserId,
                        principalTable: "BotUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BotUsers_ConsumerType_ConsumerId",
                table: "BotUsers",
                columns: new[] { "ConsumerType", "ConsumerId" });

            migrationBuilder.CreateIndex(
                name: "IX_BotUserSessions_BotUserId",
                table: "BotUserSessions",
                column: "BotUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reminds_BotUserId",
                table: "Reminds",
                column: "BotUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BotUserSessions");

            migrationBuilder.DropTable(
                name: "CronJobs");

            migrationBuilder.DropTable(
                name: "Reminds");

            migrationBuilder.DropTable(
                name: "BotUsers");
        }
    }
}
