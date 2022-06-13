using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Goblin.DataAccess.Migrations.BotDb
{
    public partial class DatetimeType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Date",
                table: "Reminds",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<int>(
                name: "CronType",
                table: "CronJobs",
                type: "integer",
                nullable: false,
                defaultValue: 4,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 2);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Reminds",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "CronType",
                table: "CronJobs",
                type: "integer",
                nullable: false,
                defaultValue: 2,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 4);
        }
    }
}
