using Microsoft.EntityFrameworkCore.Migrations;

namespace Goblin.DataAccess.Migrations.BotDb
{
    public partial class ChangeCronTimeField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                                        "Hours",
                                        "CronJobs");

            migrationBuilder.DropColumn(
                                        "Minutes",
                                        "CronJobs");

            migrationBuilder.AddColumn<string>(
                                               "Time_DayOfMonth",
                                               "CronJobs",
                                               nullable: true);

            migrationBuilder.AddColumn<string>(
                                               "Time_DayOfWeek",
                                               "CronJobs",
                                               nullable: true);

            migrationBuilder.AddColumn<string>(
                                               "Time_Hour",
                                               "CronJobs",
                                               nullable: true);

            migrationBuilder.AddColumn<string>(
                                               "Time_Minute",
                                               "CronJobs",
                                               nullable: true);

            migrationBuilder.AddColumn<string>(
                                               "Time_Month",
                                               "CronJobs",
                                               nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                                        "Time_DayOfMonth",
                                        "CronJobs");

            migrationBuilder.DropColumn(
                                        "Time_DayOfWeek",
                                        "CronJobs");

            migrationBuilder.DropColumn(
                                        "Time_Hour",
                                        "CronJobs");

            migrationBuilder.DropColumn(
                                        "Time_Minute",
                                        "CronJobs");

            migrationBuilder.DropColumn(
                                        "Time_Month",
                                        "CronJobs");

            migrationBuilder.AddColumn<int>(
                                            "Hours",
                                            "CronJobs",
                                            "integer",
                                            nullable: false,
                                            defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                                            "Minutes",
                                            "CronJobs",
                                            "integer",
                                            nullable: false,
                                            defaultValue: 0);
        }
    }
}