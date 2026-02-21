#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Goblin.DataAccess.Migrations;

public partial class DatetimeType : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<DateTimeOffset>(
                                                     "Date",
                                                     "Reminds",
                                                     "timestamp with time zone",
                                                     nullable: false,
                                                     oldClrType: typeof(DateTime),
                                                     oldType: "timestamp without time zone");

        migrationBuilder.AlterColumn<int>(
                                          "CronType",
                                          "CronJobs",
                                          "integer",
                                          nullable: false,
                                          defaultValue: 4,
                                          oldClrType: typeof(int),
                                          oldType: "integer",
                                          oldDefaultValue: 2);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<DateTime>(
                                               "Date",
                                               "Reminds",
                                               "timestamp without time zone",
                                               nullable: false,
                                               oldClrType: typeof(DateTimeOffset),
                                               oldType: "timestamp with time zone");

        migrationBuilder.AlterColumn<int>(
                                          "CronType",
                                          "CronJobs",
                                          "integer",
                                          nullable: false,
                                          defaultValue: 2,
                                          oldClrType: typeof(int),
                                          oldType: "integer",
                                          oldDefaultValue: 4);
    }
}